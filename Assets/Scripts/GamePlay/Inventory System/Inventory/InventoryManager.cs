using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryPanel;
    public ItemSlot[] itemSlots;

    public ItemSO[] itemSOs;

    [SerializeField] private Button useButton;

    public bool UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if (itemSOs[i].itemName == itemName)
            {
                bool usable = itemSOs[i].UseItem();
                return usable;
            }
        }
        return false;
    }

    private void BtnUseItem()
    {
        Debug.Log("Use Button Clicked");
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].isSelected && itemSlots[i].itemAmount > 0)
            {
                bool usable = UseItem(itemSlots[i].itemName);
                if(usable)
                {
                    itemSlots[i].itemAmount--;
                    itemSlots[i].CheckSlot();
                    return;
                }
            }
        }
    }
    
    public int AddItem(string itemName, string itemBackStory, string itemDescription, Sprite itemIcon, int itemAmount)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (!itemSlots[i].isFull && (itemSlots[i].itemName == itemName || itemSlots[i].itemAmount == 0))            
            {
                int leftOverItems = itemSlots[i].AddItem(itemName, itemBackStory, itemDescription, itemIcon, itemAmount);
                if(leftOverItems > 0)
                {
                    leftOverItems = AddItem(itemName, itemBackStory, itemDescription, itemIcon, leftOverItems);                   
                }
                return leftOverItems;
            }
        }
        return itemAmount;
    }

    public void DeselectAllSlots()
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].selectedShader.SetActive(false);
            itemSlots[i].isSelected = false;
        }
    }

    public void Start()
    {
        useButton.onClick.AddListener(BtnUseItem);
    }
}
