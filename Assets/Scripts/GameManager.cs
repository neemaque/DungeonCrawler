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
}

[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public string description;
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

