using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private GameObject player;
    private GameManager gameManager;
    private ItemDescriptionText itemDescriptionText;
    private Rigidbody2D rb;
    private int id;
    private Item item;
    void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        itemDescriptionText = GameObject.Find("ItemDescriptionText").GetComponent<ItemDescriptionText>();
    }
    public void Initialize(int id)
    {
        this.id = id;
        foreach (Item x in gameManager.items)
        {
            if (x.id == id)
            {
                item = x;
            }
        }

        float dirx = Random.Range(-1f, 1f);
        float diry = Random.Range(-1f, 1f);
        Vector2 dir = new Vector2(dirx, diry);
        float dropForce = Random.Range(300f, 500f);
        rb.AddForce(dir * dropForce);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.transform == transform)
                {
                    float dist = Vector2.Distance(player.transform.position, transform.position);
                    if (dist <= 4f)
                    {
                        Debug.Log("interacted");
                        Interact();
                    }
                }
            }
        }
        else
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.transform == transform)
                {
                    string itemDescription = item.toString();
                    itemDescriptionText.GetItem(itemDescription);
                }
            }
        }
    }

    void Interact()
    {
        player.GetComponent<Player>().AddItem(id);
        gameObject.SetActive(false);
    }
}
