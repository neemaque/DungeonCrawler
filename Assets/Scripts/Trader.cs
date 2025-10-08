using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Trader : MonoBehaviour, Interactable
{
    private GameManager gameManager;
    private Player player;
    public string type;
    private List<int> tradingItemIds;
    private bool generated = false;
    public GameObject droppedItemPrefab;
    public GameObject tradingUI;
    public Text[] tradingItemTexts;
    public Image[] tradingItemImages;
    public Button[] tradingButtons;
    public Text[] tradingButtonTexts;

    public struct Trade
    {
        public Item tradeItem;
        public int tradePrice;
        public bool buying;
    }
    private Trade[] trades;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        for (int i = 0; i < tradingButtons.Length; i++)
        {
            int index = i;
            tradingButtons[i].onClick.AddListener(() => OnButtonPressed(index));
        }
    }
    public void Interact()
    {
        if (!generated) BuildTrades();
        tradingUI.SetActive(true);
        player.busy = true;
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(trades[i].tradeItem.id + " " + trades[i].tradePrice);
        }
    }
    public void BuildTrades()
    {
        tradingItemIds = gameManager.GetItemIds(type);

        generated = true;
        trades = new Trade[10];
        for (int i = 0; i < 5; i++)
        {
            Item item = gameManager.RollItemWithIDs(tradingItemIds);
            tradingItemIds.Remove(item.id);
            int price = Mathf.RoundToInt(item.price * Random.Range(0.7f, 1.3f));
            Trade trade = new Trade
            {
                tradeItem = item,
                tradePrice = price,
                buying = false
            };
            trades[i] = trade;

            tradingItemTexts[i].text = item.name;
            tradingButtonTexts[i].text = price.ToString();
        }
        for (int i = 5; i < 10; i++)
        {
            Item item = gameManager.RollItemWithIDs(tradingItemIds);
            tradingItemIds.Remove(item.id);
            int price = Mathf.RoundToInt(item.price * Random.Range(0.3f, 0.8f));
            Trade trade = new Trade
            {
                tradeItem = item,
                tradePrice = price,
                buying = true
            };
            trades[i] = trade;

            tradingItemTexts[i].text = item.name;
            tradingButtonTexts[i].text = price.ToString();
        }
    }
    private void OnButtonPressed(int index)
    {
        if (index < 5)
        {
            if (player.coins >= trades[index].tradePrice)
            {
                GameObject droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
                droppedItem.GetComponent<DroppedItem>().Initialize(trades[index].tradeItem.id);
                player.UpdateCoin(-1 * trades[index].tradePrice);
                closeTrading();
            }
        }
        else
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i] == trades[index].tradeItem.id)
                {
                    player.UpdateCoin(trades[index].tradePrice);
                    player.DeleteItem(i);
                    closeTrading();
                    break;
                }
            }
        }
    }
    public void closeTrading()
    {
        player.busy = false;
        tradingUI.SetActive(false);
    }
}