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
    private Image image;
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        image = GetComponent<Image>();
    }
    private void Update()
    {
        UpdateSlot(player.inventory[slot]);
        if (EventSystem.current.IsPointerOverGameObject())
        {

            if (Input.GetMouseButtonDown(0))
            {
                LeftClicked();
            }
        }
    }
    public void LeftClicked()
    {
        if (isHotBar)
        {
            player.SelectSlot(slot);
            HighlightSlot();
        }
        else
        {
            int cacheId = player.inventory[player.selectedSlot];
            player.inventory[player.selectedSlot] = itemID;
            player.inventory[slot] = cacheId;
            player.SelectSlot(player.selectedSlot);
        }
    }
    public void HighlightSlot()
    {
        image.color = new Color(0f, 0f, 0f, 0.5f);
    }
    public void UpdateSlot(int id)
    {
        itemID = id;
    }
}
