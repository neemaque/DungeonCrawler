using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Trader : MonoBehaviour, Interactable
{
    private GameManager gameManager;
    private List<int> tradingItemIds;
    private bool generated = false;

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

    }
    public void Interact()
    {
        if (!generated) BuildTrades();
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(trades[i].tradeItem.id + " " + trades[i].tradePrice);
        }
    }
    public void BuildTrades()
    {
        generated = true;
        tradingItemIds = new List<int> { 501, 502, 503, 504, 505 };
        trades = new Trade[16];
        for (int i = 0; i < 2; i++)
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
        }
        for (int i = 2; i < 4; i++)
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
        }
    }
}