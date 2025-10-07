using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    public int level;
    public GameObject weaponPrefab;
    public GameObject droppedItemPrefab;
    private GameObject itemPointer;
    private GameManager gameManager;
    public int[] inventory;
    private int selectedSlot;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider saturationSlider;
    [SerializeField] private Text coinText;
    private Weapon weapon;
    public float moveSpeed = 10f;
    public int coins = 0;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int facing;
    public int health = 20;
    public int maxHealth = 20;
    public int saturation = 20;
    private bool canAttack = true;

    void Awake()
    {
        if (FindObjectsOfType<Player>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
        inventory = new int[10];
        inventory[2] = 101;

        StartCoroutine(HungerTimer());

        StartLevel();

    }
    public void NextLevel()
    {
        Debug.Log("next leveled");
        StartCoroutine(ReloadCoroutine());
        level++;
    }
    IEnumerator ReloadCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!asyncLoad.isDone)
            yield return null;

        StartLevel();
    }
    void StartLevel()
    {
        transform.position = new Vector3(3, 3, 0);

        weapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity).GetComponent<Weapon>();
        weapon.parent = transform;

        itemPointer = GameObject.Find("ItemPointer");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (itemPointer != null) itemPointer.SetActive(true);
        Debug.Log(itemPointer);

        SelectSlot(selectedSlot);
    }
    void SelectSlot(int slot)
    {
        Debug.Log("selecting slot " + slot);
        selectedSlot = slot;
        if (inventory[selectedSlot] / 100 == 1)
        {
            weapon.Unhide();
            weapon.Picked(inventory[selectedSlot]);
        }
        else
        {
            weapon.Hide();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadCoroutine());
            StartLevel();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(gameManager.RollItem().id);
        }

        
        Vector3 mousePos = new Vector3();
        if (Input.mousePosition.x < 10000) mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        mousePos.z = 0f;
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        Interactable interactable = null;
        bool foundInteractable = false;
        if (hit.collider != null)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist <= 4f)
            {
                interactable = hit.collider.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Hover();
                    foundInteractable = true;
                    if (itemPointer != null)
                    {
                        itemPointer.SetActive(true);
                        itemPointer.transform.position = mousePos;
                    }
                }
            }
        }
        if(!foundInteractable && itemPointer != null) itemPointer.SetActive(false);

        if (Input.GetMouseButtonDown(1))
        {
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
        Turn(mousePos);

        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveInput.y += 1;
            //facing = 2;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveInput.y -= 1;
            //facing = 3;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveInput.x += 1;
            //facing = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveInput.x -= 1;
            //facing = 0;
        }
        if (Input.GetMouseButtonDown(0) && inventory[selectedSlot] / 100 == 1)
        {
            if (canAttack) StartCoroutine(AttackTime(weapon.preWait, weapon.postWait));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SelectSlot(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            SelectSlot(1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            SelectSlot(2);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            SelectSlot(3);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            SelectSlot(4);
        }
        if (Input.GetKey(KeyCode.Alpha6))
        {
            SelectSlot(5);
        }
        if (Input.GetKey(KeyCode.Alpha7))
        {
            SelectSlot(6);
        }
        if (Input.GetKey(KeyCode.Alpha8))
        {
            SelectSlot(7);
        }

        weapon.facing = facing;
        moveInput = moveInput.normalized;
    }
    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
    private void Turn(Vector3 mousePos)
    {
        float x = mousePos.x - transform.position.x;
        float y = mousePos.y - transform.position.y;
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x < 0) facing = 0;
            else facing = 1;
        }
        else
        {
            if (y < 0) facing = 3;
            else facing = 2;
        }
    }
    public void TakeDamage(int amount, Vector2 direction, float knockback)
    {
        health -= amount;
        healthSlider.value = (float)health / (float)maxHealth;
    }
    public void Heal(int amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        healthSlider.value = (float)health / (float)maxHealth;
    }
    IEnumerator AttackTime(float preWait, float postWait)
    {
        weapon.Charge();
        canAttack = false;
        yield return new WaitForSeconds(preWait);
        weapon.Attack(true);
        yield return new WaitForSeconds(postWait);
        weapon.Reset();
        canAttack = true;
    }
    public void UpdateCoin(int amount)
    {
        coins += amount;
        coinText.text = "Coins: " + coins.ToString();
    }

    public void AddItem(int id)
    {
        Debug.Log("picked up item " + id);
        if (id == 1)
        {
            int amount = Random.Range(5, 10);
            UpdateCoin(amount);
            return;
        }
        DropItem();
        inventory[selectedSlot] = id;
        SelectSlot(selectedSlot);
    }
    public void DropItem()
    {
        if (inventory[selectedSlot] == 0) return;
        GameObject droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
        droppedItem.GetComponent<DroppedItem>().Initialize(inventory[selectedSlot]);
        inventory[selectedSlot] = 0;
        SelectSlot(selectedSlot);
    }
    IEnumerator HungerTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            if (saturation > 10)
            {
                int rand = Random.Range(0, 3);
                if (rand > 0)
                {
                    Heal(1);
                }
            }
            else if (saturation == 0)
            {
                int rand = Random.Range(0, 3);
                if (rand > 1)
                {
                    TakeDamage(1, Vector2.zero, 0f);
                }
            }
            saturation = Mathf.Max(saturation - 1, 0);
            saturationSlider.value = (float)saturation / 20f;
        }
    }
}