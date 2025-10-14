
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id = 101;
    public int damage = 10;
    public float range;
    public float knockback = 1f;
    public float preWait = 0.5f;
    public float postWait = 1f;
    public bool isRanged = false;
    public GameObject projectilePrefab;
    public Transform meshTransform;
    public SpriteRenderer spriteRenderer;
    public GameObject mesh;
    public int facing;
    private LayerMask hitMask;
    public Transform parent;
    private Vector2 direction;
    private Quaternion targetRotation;
    private float rotationSpeed = 200f;
    private GameManager gameManager;
    private void Awake()
    {
        hitMask = LayerMask.GetMask("NPC") | LayerMask.GetMask("Player");

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();


    }
    public void Picked(int pickedId)
    {
        id = pickedId;
        WeaponItem weaponItem = new WeaponItem();
        foreach (WeaponItem x in gameManager.weaponItems)
        {
            if (x.id == id) weaponItem = x;
        }
        if(pickedId / 100 != 1)
        {
            Item item = new Item();
            foreach (Item x in gameManager.items)
            {
                if (x.id == id) item = x;
            }
            spriteRenderer.sprite = item.sprite;
            return;
        }
        damage = weaponItem.damage;
        range = weaponItem.range;
        knockback = weaponItem.knockback;
        preWait = weaponItem.preWait;
        postWait = weaponItem.postWait;
        isRanged = weaponItem.isRanged;
        spriteRenderer.sprite = weaponItem.sprite;
    }
    public void Update()
    {
        if (facing == 0) direction = new Vector2(-1, 0);
        if (facing == 1) direction = new Vector2(1, 0);
        if (facing == 2) direction = new Vector2(0, 1);
        if (facing == 3) direction = new Vector2(0, -1);

        if (facing == 1)spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;

        transform.position = parent.position + (Vector3)direction / 2;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
    public void Attack(bool isPlayer, float strengthMultiplier)
    {
        if (direction.x > 0) targetRotation = Quaternion.Euler(0, 0, -100);
        else targetRotation = Quaternion.Euler(0, 0, 100);
        rotationSpeed = 1000f;
        Vector2 attackPos = (Vector2)transform.position;
        float rangeUp = range;
        if (isPlayer) rangeUp = range + 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, rangeUp, hitMask);

        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (hit.transform == parent) continue;
                damageable.TakeDamage((int)(damage * strengthMultiplier), direction, knockback);
            }
        }
    }
    public void RangedAttack(Vector2 direction)
    {
        if (direction.x > 0) targetRotation = Quaternion.Euler(0, 0, -100);
        else targetRotation = Quaternion.Euler(0, 0, 100);
        rotationSpeed = 1000f;

        Projectile projectile = Instantiate(projectilePrefab, new Vector3(transform.position.x + direction.x * 0.5f, transform.position.y + direction.y * 0.5f, transform.position.z), Quaternion.identity).GetComponent<Projectile>();
        projectile.Shot(direction, 10f, damage, knockback, parent);
        
    }
    public void Charge()
    {
        if (direction.x > 0) targetRotation = Quaternion.Euler(0, 0, 60);
        else targetRotation = Quaternion.Euler(0, 0, -60);
        rotationSpeed = 100f;
    }
    public void Reset()
    {
        targetRotation = Quaternion.Euler(0, 0, 0);
        rotationSpeed = 1000f;
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
    public void Unhide()
    {
        //mesh.SetActive(true);
    }
    public void Hide()
    {
        //mesh.SetActive(false);
    }
}
public interface IDamageable
{
    void TakeDamage(int amount, Vector2 direction, float knockback);
}