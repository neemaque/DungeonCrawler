using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    private int damage;
    private float knockback;
    private Vector2 direction;
    private float moveSpeed;
    private Transform parent;

    public void Shot(Vector2 direction, float moveSpeed, int damage, float knockback, Transform parent)
    {
        this.direction = direction;
        this.moveSpeed = moveSpeed;
        this.damage = damage;
        this.knockback = knockback;
        this.parent = parent;
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = direction * moveSpeed;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == parent) return;
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, Vector2.zero, knockback);
            Debug.Log("damaged with projectile!");
        }
        gameObject.SetActive(false);
    }
}
