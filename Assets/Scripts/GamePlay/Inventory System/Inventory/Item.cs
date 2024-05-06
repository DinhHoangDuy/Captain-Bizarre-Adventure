using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [TextArea] [SerializeField] private string itemDescription;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private int itemAmount;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("/GameManagers/PlayerConsoleManager").GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inventoryManager.AddItem(itemName, itemDescription, itemIcon, itemAmount);
            Destroy(gameObject);
        }
    }
}