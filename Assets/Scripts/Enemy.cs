using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IDamageable
{
    public int id = 1;
    public string name;
    public int health = 20;
    public int maxHealth = 20;
    public float range;
    public float attentionSpan = 3f;
    private float attention;
    public float moveSpeed = 3f;
    public int weaponOfChoice = 101;
    public GameObject weaponPrefab;
    public GameObject droppedItemPrefab;
    [SerializeField] private Text nameText;
    [SerializeField] private Slider healthSlider;
    private List<int> inventory;
    private Weapon weapon;
    private GameObject player;
    private Transform lastSaw;
    private Vector3 target;
    private int facing;
    private string phase;
    private Rigidbody2D rb;
    private bool canAttack = true;
    private bool canMove = true;
    private bool dead = false;
    
    void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        phase = "idle";

        health = maxHealth;
        player = GameObject.Find("Player");
    }
    public void Initialize(NPC npc)
    {
        id = npc.id;
        name = npc.name;
        maxHealth = npc.maxHealth;
        health = maxHealth;
        moveSpeed = npc.moveSpeed;
        weaponOfChoice = npc.weaponOfChoice;
        range = npc.range;

        nameText.text = name;
        
        BuildInventory();
        weapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity).GetComponent<Weapon>();
        weapon.parent = transform;
        weapon.Picked(weaponOfChoice);
    }
    private void BuildInventory()
    {
        inventory = new List<int>();
        int numberOfCoins = Random.Range(2, 5);
        for (int i = 0; i < numberOfCoins; i++)
        {

            inventory.Add(1);
        }
        int rand = Random.Range(0, 1);
        if (rand == 0)
        {
            inventory.Add(weaponOfChoice);
        }
    }
    private void Update()
    {
        weapon.facing = facing;
        if (Vector3.Distance(transform.position, player.transform.position) <= range)
        {
            LayerMask mask = LayerMask.GetMask("Wall");
            Vector2 dir = (player.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, range, mask | (1 << player.gameObject.layer));

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("player visible!");
                    phase = "chase";
                    target = new Vector3(player.transform.position.x, player.transform.position.y, 0);
                    attention = attentionSpan;

                    if(weapon.isRanged && canAttack && !dead)
                    {
                        Vector2 dir1 = (target - transform.position).normalized;
                        LookAt(dir1);
                        canAttack = false;
                        Debug.Log("starting attack!");
                        StartCoroutine(AttackTime(weapon.preWait, weapon.postWait));
                    }
                }
            }
        }
        if (phase == "chase" && !dead)
        {
            attention -= Time.deltaTime;
            if (attention <= 0)
            {
                phase = "idle";
                attention = 0;
            }
            if (Vector2.Distance(target, transform.position) < weapon.range + 1 && canAttack && !weapon.isRanged)
            {
                StartCoroutine(AttackTime(weapon.preWait, weapon.postWait));
            }
        }
    }
    void FixedUpdate()
    {
        if (dead)
        {
            return;
        }
        if (phase == "chase" && canMove && Vector2.Distance(target, transform.position) > 0.1)
        {
            Vector2 dir = (target - transform.position).normalized;
            LookAt(dir);
            rb.linearVelocity = dir * moveSpeed;
        }
        else if (Vector2.Distance(target, transform.position) <= 0.1 && canMove)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
        else if (phase == "idle")
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
    }
    public void TakeDamage(int amount, Vector2 direction, float knockback)
    {
        health -= amount;
        healthSlider.value = (float)health / (float)maxHealth;
        StartCoroutine(DamageCooldown(direction, knockback));
        if (health <= 0 && !dead) Death();
    }
    private void LookAt(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) facing = 1;
            else facing = 0;
        }
        else
        {
            if (dir.y > 0) facing = 2;
            else facing = 3;
        }
    }
    IEnumerator AttackTime(float preWait, float postWait)
    {
        weapon.Charge();
        canAttack = false;
        canMove = false;
        yield return new WaitForSeconds(preWait);

        if(weapon.isRanged)
        {
            Vector2 projDirection = new Vector2(target.x - transform.position.x, target.y - transform.position.y);
            projDirection = projDirection.normalized;
            
            if(!dead)weapon.RangedAttack(projDirection);
        }
        else weapon.Attack(false);
        
        canMove = true;
        yield return new WaitForSeconds(postWait/2);
        weapon.Reset();
        yield return new WaitForSeconds(postWait/2);
        canAttack = true;
    }
    IEnumerator DamageCooldown(Vector2 direction, float knockback)
    {
        canMove = false;
        canAttack = false;
        rb.linearVelocity = new Vector2(0, 0);
        rb.AddForce(direction * knockback * 50);
        yield return new WaitForSeconds(1f);
        canMove = true;
        canAttack = true;
    }
    private void Death()
    {
        Debug.Log("im dead!");
        canMove = false;
        canAttack = false;
        dead = true;
        rb.freezeRotation = false;
        weapon.Deactivate();
        StartCoroutine(DropItems());
    }
    IEnumerator DropItems()
    {
        foreach(int id in inventory)
        {
            GameObject droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
            droppedItem.GetComponent<DroppedItem>().Initialize(id);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
