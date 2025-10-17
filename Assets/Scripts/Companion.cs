using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Companion : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 2f;
    public SpriteRenderer spriteRenderer;
    private GameObject player;
    private Player playerr;
    private Vector3 offset;
    private int timer;
    private int timer2;
    public Text text;
    void Start()
    {
        offset = new Vector3(-1f, 1f, 0);
        player = GameObject.Find("Player");
        playerr = player.GetComponent<Player>();
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
        timer2++;
        if(timer2 > 2500)
        {
            if(playerr.health < 3)
            {
                int rand = Random.Range(0,3);
                if(rand == 0)StartCoroutine(Say("careful!"));
                if(rand == 1)StartCoroutine(Say("im scared.."));
                if(rand == 2)StartCoroutine(Say("you're low!"));
            }
            else if(playerr.saturation < 3)
            {
                int rand = Random.Range(0,2);
                if(rand == 0)StartCoroutine(Say("eat something..."));
                if(rand == 1)StartCoroutine(Say("it's time to eat!"));
            }
            else
            {
                int rand = Random.Range(0,14);
                if(rand == 0)StartCoroutine(Say("i love you"));
                if(rand == 1)StartCoroutine(Say("^_^"));
                if(rand == 2)StartCoroutine(Say(":P"));
                if(rand == 3)StartCoroutine(Say("you're very brave"));
                if(rand == 4)StartCoroutine(Say("it's ok if you fail"));
                if(rand == 5)StartCoroutine(Say("eat to regenerate health"));
                if(rand == 6)StartCoroutine(Say("i'm here"));
                if(rand == 7)StartCoroutine(Say("take your time"));
                if(rand == 8)StartCoroutine(Say("la la la la"));
                if(rand == 9)StartCoroutine(Say("i'm not feeling it... not feeling it..."));
                if(rand == 10)StartCoroutine(Say("it's all you"));
                if(rand == 11)StartCoroutine(Say("for me it's only you"));
                if(rand == 12)StartCoroutine(Say("i'll follow wherever you go!"));  
                if(rand == 13)StartCoroutine(Say("<3"));
            }
            timer2 = 0;
        }
    }
    void Update()
    {

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
    IEnumerator Say(string sentence)
    {
        int rand = Random.Range(0,2);
        if(rand == 0)text.text = sentence;
        yield return new WaitForSeconds(4f);
        text.text = "";
    }
}
