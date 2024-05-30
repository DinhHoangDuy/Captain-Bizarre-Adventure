using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemSO itemSO;
    private Sprite itemIcon;
    [SerializeField] private int itemAmount;
    private string itemName;
    private string itemBackStory;
    private string itemDescription;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("/Player UI").GetComponent<InventoryManager>();
        itemName = itemSO.itemName;
        itemBackStory = itemSO.itemBackStory;
        itemDescription = itemSO.itemDescription;
        itemIcon = itemSO.itemIcon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int leftOverItems = inventoryManager.AddItem(itemName, itemBackStory, itemDescription, itemIcon, itemAmount);
            if(leftOverItems <= 0)
                Destroy(gameObject);
            else
                itemAmount = leftOverItems;
        }
    }
}