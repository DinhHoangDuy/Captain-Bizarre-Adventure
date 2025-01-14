using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpansionChipManager : MonoBehaviour
{
    public static ExpansionChipManager instance;
    private ExpansionChipStatus expansionChipStatus;

    [Header("Panel Components")]
    [SerializeField] private GameObject expansionChipPanel;
    [SerializeField] private ExpansionChipSlot[] expansionChipSlots;
    [SerializeField] private Button equipButton;
    [SerializeField] private TextMeshProUGUI loadIndicatorNumber;
    [SerializeField] private TextMeshProUGUI loadIndicatorText;
    [SerializeField] private int maxLoad = 10;
    [SerializeField] private int maxChipAmount = 6;
    private int currentLoad = 0;
    private int currentChipAmount = 0;

    [Header("Add Chip Name, Chip Icon Image and Chip Description from the Description Panel here")]
    [SerializeField] private Image chipIconDescription;
    public Sprite blankChipIcon;
    [SerializeField] private TextMeshProUGUI  chipNameDescriptionPanel;
    [SerializeField] private TextMeshProUGUI chipDescriptionText;

    // Manager State
    private bool anySlotSelected = false;
    // private bool isAllowedToEquip = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        expansionChipStatus = GetComponent<ExpansionChipStatus>();
        expansionChipPanel.SetActive(false);

        DeselectAllSlots();
        DeleteDescription();
        equipButton.onClick.AddListener(ToggleEquipButton);
    }
    private void Update()
    {
        if (InputManager.instance.expansionChipPanelInputTriggered)
        {
            expansionChipPanel.SetActive(!expansionChipPanel.activeSelf);
            InputManager.instance.expansionChipPanelInputTriggered = false;
        }

        // Update Button Text based on the selected Chip
        for (int i = 0; i < expansionChipSlots.Length; i++)
        {
            // Initialize variables for button text and interactability
            string buttonText = "Equip";
            bool buttonInteractable = true;

            // Pre-calculate frequently used values
            bool isLocked = expansionChipSlots[i].isLocked;
            bool isEquipped = expansionChipSlots[i].isEquipped;
            int chipLoadData = expansionChipSlots[i].chipLoadData;

            // Check if the slot is selected
            if (expansionChipSlots[i].isSelected)
            {
                anySlotSelected = true;

                if (isLocked)
                {
                    buttonText = "Locked";
                    buttonInteractable = false;
                }
                else if (isEquipped)
                {
                    buttonText = "Unequip";
                }
                else
                {
                    if (ExpansionChipStatus.instance.isOverclocked)
                    {
                        if (chipLoadData == 0 || (expansionChipStatus.isKeyOfBloodMoonEquipped && currentChipAmount < maxChipAmount))
                        {
                            buttonText = "Equip";
                        }
                        else
                        {
                            buttonText = "Equip is not allowed";
                            buttonInteractable = false;
                        }
                    }
                    else
                    {
                        int futureLoad = currentLoad + chipLoadData;
                        int futureChipAmount = currentChipAmount + (chipLoadData > 0 ? 1 : 0);

                        if (expansionChipStatus.isKeyOfBloodMoonEquipped)
                        {
                            if (futureChipAmount > maxChipAmount)
                            {
                                buttonText = "Overloaded!";
                                buttonInteractable = false;
                            }
                        }
                        else
                        {
                            if (futureLoad > maxLoad)
                            {
                                buttonText = "Overclocked!";
                                buttonInteractable = false;
                            }
                        }
                    }
                }

                // Apply the calculated text and interactability to the button
                equipButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
                equipButton.interactable = buttonInteractable;
            }        
        }
        equipButton.gameObject.SetActive(anySlotSelected);

        #region Update Chip Load/Amount
        if(!expansionChipStatus.isKeyOfBloodMoonEquipped)
        { 
            if(currentLoad > maxLoad)
            {
                ExpansionChipStatus.instance.isOverclocked = true;
            }
            else
            {
                ExpansionChipStatus.instance.isOverclocked = false;
            }
        }
        else 
        {
            // Key of Blood Moon is equipped
            ExpansionChipStatus.instance.isOverclocked = true; // Overclock state is forced to be active when the Key of Blood Moon is equipped
        }

        UpdateLoadIndicator();
        UpdateLoadIndicatorText();
        #endregion
    }

    private void ToggleEquipButton()
    {
        for(int i = 0; i < expansionChipSlots.Length; i++)
        {
            if (expansionChipSlots[i].isSelected)
            {
                if(expansionChipSlots[i].isLocked)
                {
                    return;
                }
                if (expansionChipSlots[i].isEquipped)
                {
                    // This is to unequip the selected Chip
                    expansionChipSlots[i].isEquipped = false;
                    expansionChipSlots[i].equippedShader.enabled = false;

                    ChangeChipLoad(-expansionChipSlots[i].chipLoadData);
                
                    if(expansionChipSlots[i].chipLoadData > 0)
                    {
                        ChangeChipAmount(-1);
                    }
                    else
                    {
                        ChangeChipAmount(0);
                    }
                    return;
                }
                else
                {
                    // This is to equip the selected Chip
                    expansionChipSlots[i].isEquipped = true;
                    expansionChipSlots[i].equippedShader.enabled = true;

                    ChangeChipLoad(expansionChipSlots[i].chipLoadData);
                    if(expansionChipSlots[i].chipLoadData > 0)
                    {
                        ChangeChipAmount(1);
                    }
                    else
                    {
                        ChangeChipAmount(0);
                    }
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
        chipIconDescription.sprite = blankChipIcon;
        chipNameDescriptionPanel.text = "";
        chipDescriptionText.text = "";
    }

    #region Load Indicator
    public void UpdateLoadIndicator()
    {
        if(expansionChipStatus.isKeyOfBloodMoonEquipped)
        {
            loadIndicatorNumber.text = (currentChipAmount) + "/" + maxChipAmount;
        }
        else
        {
            loadIndicatorNumber.text = currentLoad + "/" + maxLoad;
        }
    }
    public void UpdateLoadIndicatorText()
    {
        if (expansionChipStatus.isKeyOfBloodMoonEquipped)
        {
            loadIndicatorText.text = "Key of Blood Moon is equipped. Current Chip Amount: ";
        }
        else
        {
            loadIndicatorText.text = "Current Chip Load: ";
        }
    }
    public void ChangeChipLoad(int loadChange)
    {
        currentLoad += loadChange;
    }
    public void ChangeChipAmount(int amount)
    {
        currentChipAmount += amount;
    }
    #endregion
}
