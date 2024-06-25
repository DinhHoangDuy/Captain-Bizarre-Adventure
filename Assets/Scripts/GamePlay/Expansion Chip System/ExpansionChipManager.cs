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
    private bool isAllowedToEquip = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();

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
                    if (isAllowedToEquip)
                    {
                        equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                        equipButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    else
                    {
                        equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip is not allowed";
                        equipButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    }
                }
            }
        }
        equipButton.gameObject.SetActive(anySlotSelected);

        #region Update Chip Load/Amount
        if(currentLoad > maxLoad && !expansionChipStatus.isKeyOfBloodMoonEquipped)
        {
            expansionChipStatus.isOverclocked = true;
            isAllowedToEquip = false;
        }
        else
        {
            expansionChipStatus.isOverclocked = false;
            isAllowedToEquip = true;
        }

        if(currentChipAmount > maxChipAmount && expansionChipStatus.isKeyOfBloodMoonEquipped)
        {
            expansionChipStatus.isChipAmountBreach = true;
            isAllowedToEquip = false;
        }
        else
        {
            expansionChipStatus.isChipAmountBreach = false;
            isAllowedToEquip = true;
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
                    Debug.Log("Chip is locked");
                    return;
                }
                if (expansionChipSlots[i].isEquipped)
                {
                    // This is to unequip the selected Chip
                    expansionChipSlots[i].isEquipped = false;
                    expansionChipSlots[i].equippedShader.enabled = false;

                    ChangeChipLoad(-expansionChipSlots[i].chipLoadData);
                    
                    if(expansionChipSlots[i].chipNameData == "Key of Blood Moon")
                    {
                        ChangeChipAmount(0);
                    }
                    else if(expansionChipSlots[i].chipLoadData > 0)
                    {
                        ChangeChipAmount(-1);
                    }
                    else
                    {
                        Debug.Log("Chip Amount is not counted for this chip: " + expansionChipSlots[i].chipNameData);
                    }
                    return;
                }
                else
                {
                    // This is to equip the selected Chip
                    expansionChipSlots[i].isEquipped = true;
                    expansionChipSlots[i].equippedShader.enabled = true;

                    ChangeChipLoad(expansionChipSlots[i].chipLoadData);

                    if(expansionChipSlots[i].chipNameData == "Key of Blood Moon")
                    {
                        ChangeChipAmount(0);
                    }
                    else if(expansionChipSlots[i].chipLoadData > 0)
                    {
                        ChangeChipAmount(1);
                    }
                    else
                    {
                        Debug.Log("Chip Amount is not counted for this chip: " + expansionChipSlots[i].chipNameData);
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
