using UnityEngine;
using System.Collections;

public class Boulder : MonoBehaviour
{
    private Vector2 dir = new Vector2();
    private Rigidbody2D rb;
    private bool moving = true;
    private float speed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        int rand = Random.Range(0, 4);
        if(rand == 0)dir = new Vector2(-1, 0);
        if(rand == 1)dir = new Vector2(1, 0);
        if(rand == 2)dir = new Vector2(0, 1);
        if(rand == 3)dir = new Vector2(0, -1);
        StartCoroutine(FallTimer());
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null && moving)
        {
            damageable.TakeDamage(20, dir, 50f);
            Debug.Log("damaged with boulder!");
            speed = Mathf.Max(0, speed - 1f);
        }
        else moving = false;
    }
    private void FixedUpdate()
    {
        if(moving)rb.linearVelocity = dir * speed;
    }
    IEnumerator FallTimer()
    {
        speed = 0f;
        yield return new WaitForSeconds(0.5f);
        speed = 3f;
    }
}
