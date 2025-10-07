using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chest : MonoBehaviour, Interactable
{
    private GameObject player;
    private GameManager gameManager;
    
    public GameObject droppedItemPrefab;
    private List<int> inventory;
    private bool emptied = false;
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        inventory = new List<int>();
        
    }
    private void BuildInventory()
    {
        int numberOfItems = Random.Range(0, 3);
        while (numberOfItems > 0)
        {
            numberOfItems--;
            inventory.Add(gameManager.RollItem().id);   
        }
    }
    void Update()
    {
        
    }
    public void Interact()
    {
        if (emptied) return;
        BuildInventory();
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
