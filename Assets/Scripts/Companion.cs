using UnityEngine;
using System.Collections;

public class Companion : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 2f;
    public SpriteRenderer spriteRenderer;
    private GameObject player;
    private Vector3 offset;
    private int timer;
    void Start()
    {
        offset = new Vector3(-1f, 1f, 0);
        player = GameObject.Find("Player");
        target = player.transform;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        timer++;
        if (timer > 1000)
        {
            Debug.Log("try for");
            int rand = Random.Range(0, 5);
            if (rand == 0)
            {
                Debug.Log("coming closer");
                StartCoroutine(ComeClose());
            }
            timer = 0;
        }
    }
    IEnumerator ComeClose()
    {
        offset.x = 0f;
        offset.y = 0f;
        smoothSpeed = 4f;
        spriteRenderer.sortingOrder = 6;
        yield return new WaitForSeconds(2);

        smoothSpeed = 2f;
        offset.x = -1f;
        offset.y = 1f;

        yield return new WaitForSeconds(0.5f);
        
        spriteRenderer.sortingOrder = 4;
    }
}
