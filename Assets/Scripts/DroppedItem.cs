using UnityEngine;

public class DroppedItem : MonoBehaviour, Interactable
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
        float dropForce = Random.Range(150f, 250f);
        rb.AddForce(dir * dropForce);
    }

    public void Interact()
    {
        player.GetComponent<Player>().AddItem(id);
        gameObject.SetActive(false);
    }
    public void Hover()
    {
        string itemDescription = item.toString();
        itemDescriptionText.GetItem(itemDescription);
    }
}
