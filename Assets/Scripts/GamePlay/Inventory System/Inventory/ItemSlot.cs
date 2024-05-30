using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Item Data
    public string itemName;
    private string itemDescription;
    private Sprite itemIcon;
    public int itemAmount;
    public bool isFull = false;
    public Sprite emptySlotIcon;
    // Item Slot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image iconImage;

    // Item Description
    public Image itemImage;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    public GameObject selectedShader;
    public bool isSelected = false;
    private InventoryManager inventoryManager;
    private int maxQuantity = 99;

    private void Start()
    {
        inventoryManager = GameObject.Find("/Player UI").GetComponent<InventoryManager>();
    }
    private void Update()
    {
        if(itemAmount <= 0)
        {
            quantityText.text = "";
        }
        else
            quantityText.text = itemAmount.ToString();
    }
    public void CheckSlot()
    {
        if(itemAmount <= 0)
        {
            EmptySlot();
        }
    }

    private void EmptySlot()
    {
        itemName = "";
        itemDescription = "";
        itemIcon = null;
        itemAmount = 0;
        isFull = false;
        iconImage.sprite = emptySlotIcon;
        quantityText.text = "";
        itemImage.sprite = emptySlotIcon;
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public int AddItem(string itemName,string itemBackStory, string itemDescription, Sprite itemIcon, int itemAmount)
    {
        if(isFull)
        {
            return itemAmount;
        }

        // Update the Item Details
        // Add Item Name
        this.itemName = itemName;

        // Add Item Description
        this.itemDescription = itemBackStory + "\n" + itemDescription;

        // Add Item Icon
        this.itemIcon = itemIcon;        
        iconImage.sprite = itemIcon;


        // Add Item Amount
        this.itemAmount += itemAmount;
        if(this.itemAmount >= maxQuantity)
        {
            quantityText.text = maxQuantity.ToString();
            isFull = true;
            int extraItems = this.itemAmount - maxQuantity;
            this.itemAmount = maxQuantity;
            return extraItems;
        }
        else
        {
            quantityText.text = this.itemAmount.ToString();
            return 0;
        }       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        isSelected = true;

        if(itemIcon == null)
        {
            itemImage.sprite = emptySlotIcon;
            itemNameText.text = "";
            itemDescriptionText.text = "";
        }
        else
        {
            itemImage.sprite = itemIcon;
            itemNameText.text = itemName;
            itemDescriptionText.text = itemDescription;
        }      
    }
}
