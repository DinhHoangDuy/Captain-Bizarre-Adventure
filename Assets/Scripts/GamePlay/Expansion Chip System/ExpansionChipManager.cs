using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpansionChipManager : MonoBehaviour
{
    public static ExpansionChipManager instance;
    [SerializeField] private ExpansionChipSlot[] expansionChipSlots;
    [SerializeField] private Button equipButton;

    [Header("Add Chip Name, Chip Icon Image and Chip Description from the Description Panel here")]
    [SerializeField] private Image chipIconDescription;
    public Sprite blankChipIcon;
    [SerializeField] private TextMeshProUGUI  chipNameDescriptionPanel;
    [SerializeField] private TextMeshProUGUI chipDescriptionText;

    // Manager State
    private bool anySlotSelected = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        DeselectAllSlots();
        DeleteDescription();
        equipButton.onClick.AddListener(ToggleEquipButton);
    }
    private void Update()
    {
        // Update Button Text based on the selected Chip
        for (int i = 0; i < expansionChipSlots.Length; i++)
        {
            if (expansionChipSlots[i].isSelected)
            {
                anySlotSelected = true;
                if(expansionChipSlots[i].isLocked)
                {
                    equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Locked";
                    // Change the button color to gray
                    equipButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
                else if (expansionChipSlots[i].isEquipped)
                {
                    equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
                    equipButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
                else if (!expansionChipSlots[i].isEquipped)
                {
                    equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                    equipButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
        equipButton.gameObject.SetActive(anySlotSelected);
    }

    private void ToggleEquipButton()
    {
        for(int i = 0; i < expansionChipSlots.Length; i++)
        {
            if (expansionChipSlots[i].isSelected)
            {
                if(expansionChipSlots[i].isLocked)
                {
                    Debug.Log("Chip is locked");
                    return;
                }
                if (expansionChipSlots[i].isEquipped)
                {
                    expansionChipSlots[i].isEquipped = false;
                    expansionChipSlots[i].equippedShader.enabled = false;
                    Debug.Log("Unequipped Chip: " + expansionChipSlots[i].chipNameData);
                    return;
                }
                else
                {
                    expansionChipSlots[i].isEquipped = true;
                    expansionChipSlots[i].equippedShader.enabled = true;
                    Debug.Log("Equipped Chip: " + expansionChipSlots[i].chipNameData);
                    return;
                }
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < expansionChipSlots.Length; i++)
        {
            expansionChipSlots[i].isSelected = false;
        }
        anySlotSelected = false;
    }
    public void UnlockChip(ExpansionChipSO chipData)
    {
        Debug.Log("Unlocking chip: " + chipData.chipName);
        for (int i = 0; i < expansionChipSlots.Length; i++)
        {
            if (expansionChipSlots[i].chipData == chipData)
            {
                expansionChipSlots[i].isLocked = false;
                return;
            }
        }
    }
    public void DeleteDescription()
    {
        Debug.Log("Deleting Description Panel");
        chipIconDescription.sprite = blankChipIcon;
        chipNameDescriptionPanel.text = "";
        chipDescriptionText.text = "";
    }
}
