using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public List<Item> items;
    public List<WeaponItem> weaponItems;
    public List<FoodItem> foodItems;
    public List<MiscItem> miscItems;
    public List<NPC> npcs;
    private void Awake()
    {
        items = new List<Item>();
        foreach (WeaponItem x in weaponItems)
        {
            items.Add(x);
        }
        foreach (FoodItem x in foodItems)
        {
            items.Add(x);
        }
        foreach (MiscItem x in miscItems)
        {
            items.Add(x);
        }
    }
    public List<int> GetItemIds(string type)
    {
        List<int> ids = new List<int>();
        if (type == "all" || type == "")
        {
            foreach (Item x in items)
            {
                ids.Add(x.id);
            }
        }
        else if (type == "weapons")
        {
            foreach (Item x in weaponItems)
            {
                ids.Add(x.id);
            }
        }
        else if (type == "foods")
        {
            foreach (Item x in foodItems)
            {
                ids.Add(x.id);
            }
        }
        else if (type == "bakery")
        {
            foreach (FoodItem x in foodItems)
            {
                if (x.isBakedGood) ids.Add(x.id);
            }
        }
        return ids;
    }
    public Item RollItem()
    {
        int totalWeight = 0;
        foreach (Item x in items)
        {
            totalWeight += x.rarity;
        }
        int rand = Random.Range(0, totalWeight + 1);

        foreach (Item x in items)
        {
            if (rand < x.rarity)
            {
                return x;
            }
            rand -= x.rarity;
        }
        return items[0];
    }
    public Item RollItemWithIDs(List<int> ids)
    {
        List<Item> listOfItems = new List<Item>();
        foreach (int id in ids)
        {
            foreach (Item x in items)
            {
                if (x.id == id)
                {
                    listOfItems.Add(x);
                    break;
                }
            }
        }
        int totalWeight = 0;
        foreach (Item x in listOfItems)
        {
            totalWeight += x.rarity;
        }
        int rand = Random.Range(0, totalWeight + 1);

        foreach (Item x in listOfItems)
        {
            if (rand < x.rarity)
            {
                return x;
            }
            rand -= x.rarity;
        }
        return items[0];
    }
}

[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public string description;
    public int rarity;
    public int price;
    public Sprite sprite;
    public virtual string toString()
    {
        return name;
    }
}

[System.Serializable]
public class WeaponItem : Item
{
    public int damage = 5;
    public float range = 0.5f;
    public float knockback = 15f;
    public float preWait = 0.5f;
    public float postWait = 1f;
    public override string toString()
    {
        return name + "\n " + description + "\nDamage: " + damage + "\nRange: " + range + "\nKnockback: " + knockback;
    }
}

[System.Serializable]
public class FoodItem : Item
{
    public int saturation;
    public bool isBakedGood;
    public override string toString()
    {
        return name + "\n " + description + "\nSaturation: " + saturation;
    }
}

[System.Serializable]
public class MiscItem : Item
{
    public override string toString()
    {
        return name + "\n " + description;
    }
}

[System.Serializable]
public class NPC
{
    public int id;
    public string name;
    public string description;
    public int maxHealth = 20;
    public float moveSpeed = 3f;
    public int weaponOfChoice = 101;
    public float range = 10f;
    public int gang = 1;
    public Sprite sprite;
}

