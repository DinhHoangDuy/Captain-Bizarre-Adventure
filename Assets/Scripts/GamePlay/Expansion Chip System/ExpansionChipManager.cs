using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpansionChipManager : MonoBehaviour
{
    public static ExpansionChipManager instance;
    [SerializeField] private ExpansionChipSlot[] expansionChipSlots;
    private ExpansionChipSO[] expansionChipSOs;

    [Header("Add Chip Name, Chip Icon Image and Chip Description from the Description Panel here")]
    [SerializeField] private Image chipIconDescription;
    public Sprite blankChipIcon;
    [SerializeField] private TextMeshProUGUI  chipNameDescriptionPanel;
    [SerializeField] private TextMeshProUGUI chipDescriptionText;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < expansionChipSlots.Length; i++)
        {
            if(expansionChipSlots[i] == null)
            {
                Debug.LogWarning("Expansion Chip Slot is not set on " + gameObject.name);
                continue;
            }
            else
            {
                expansionChipSOs[i] = expansionChipSlots[i].chipData;
            }
        }

        DeselectAllSlots();
        DeleteDescription();
    }
    public void DeselectAllSlots()
    {
        for (int i = 0; i < expansionChipSlots.Length; i++)
        {
            expansionChipSlots[i].isSelected = false;
        }
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
