using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chest : MonoBehaviour, Interactable
{
    private GameObject player;
    public GameObject droppedItemPrefab;
    private List<int> inventory;
    private bool emptied = false;
    private void Awake()
    {
        player = GameObject.Find("Player");
        inventory = new List<int>();
        BuildInventory();
    }
    private void BuildInventory()
    {
        inventory.Add(1);
        inventory.Add(102);
    }
    void Update()
    {
        
    }
    public void Interact()
    {
        if (emptied) return;
        Debug.Log("interacted chest");
        emptied = true;
        StartCoroutine("DropItems");
    }
    public void Hover()
    {
        
    }
    IEnumerator DropItems()
    {
        foreach (int id in inventory)
        {
            GameObject droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
            droppedItem.GetComponent<DroppedItem>().Initialize(id);

            yield return new WaitForSeconds(0.5f);
        }
        gameObject.SetActive(false);
    }
}
