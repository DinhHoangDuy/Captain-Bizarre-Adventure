using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using JetBrains.Annotations;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Item Data
    private string itemName;
    public string _itemName { get { return itemName; } }
    private string itemDescription;
    private Sprite itemIcon;
    private int itemAmount;
    public int _itemAmount { get { return itemAmount; } }
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
        inventoryManager = GameObject.Find("/GameManagers/PlayerConsoleManager").GetComponent<InventoryManager>();
    }

    public void AddItem(string itemName, string itemDescription, Sprite itemIcon, int itemAmount)
    {
        // Add Item Name
        this.itemName = itemName;
        // Add Item Description
        this.itemDescription = itemDescription;
        // Add Item Icon
        this.itemIcon = itemIcon;        
        iconImage.sprite = itemIcon;
        // Add Item Amount
        this.quantityText.text = itemAmount.ToString();
        isFull = true;

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
