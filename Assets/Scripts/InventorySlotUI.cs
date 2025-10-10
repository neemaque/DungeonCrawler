using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour
{
    public int slot;
    public bool isHotBar;
    public int itemID;
    private Player player;
    private GameManager gameManager;
    public Image image;
    public Image backgroundImage;
    public Text stackAmount;
    public GameObject pointer;
    private Item item;
    private ItemDescriptionText itemDescriptionText;
    private bool hovered;
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        itemDescriptionText = GameObject.Find("ItemDescriptionText").GetComponent<ItemDescriptionText>();
        image.preserveAspect = true;
    }
    private void Update()
    {
        UpdateSlot(player.inventory[slot]);
        if (player.selectedSlot == slot) pointer.SetActive(true);
        else pointer.SetActive(false);

        if(hovered)
        {
            if (itemID != 0)
            {
                string itemDescription = item.toString();
                itemDescriptionText.GetItem(itemDescription);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                player.DropItem(slot);
            }
        }
    }
    public void LeftClicked()
    {
        backgroundImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        if (player.lastClickedSlot == -1)
        {
            player.lastClickedSlot = slot;
        }
        else
        {
            int cacheId = player.inventory[player.lastClickedSlot];
            int cacheStack = player.inventoryStacks[player.lastClickedSlot];
            player.inventory[player.lastClickedSlot] = itemID;
            player.inventoryStacks[player.lastClickedSlot] = player.inventoryStacks[slot];
            player.inventory[slot] = cacheId;
            player.inventoryStacks[slot] = cacheStack;
            player.SelectSlot(player.selectedSlot);
            player.lastClickedSlot = -1;
        }
    }
    public void Hover()
    {
        backgroundImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        hovered = true;
        
    }
    public void UnHover()
    {
        backgroundImage.color = new Color(0f, 0f, 0f, 0.5f);
        hovered = false;
    }
    private void OnEnable()
    {
        UnHover();
    }
    public void UpdateSlot(int id)
    {
        itemID = id;
        stackAmount.text = player.inventoryStacks[slot].ToString();
        if (id != 0)
        {
            item = gameManager.FindItem(itemID);
            image.sprite = item.sprite;
            image.color = new Color(1f, 1f, 1f, 1.0f);
        }
        else
        {
            image.color = new Color(1f, 1f, 1f, 0.0f);
        }

        if (player.inventoryStacks[slot] < 2)
        {
            stackAmount.gameObject.SetActive(false);
        }
        else stackAmount.gameObject.SetActive(true);
    }
}
