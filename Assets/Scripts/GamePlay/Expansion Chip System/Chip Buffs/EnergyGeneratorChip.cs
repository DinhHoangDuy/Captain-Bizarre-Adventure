using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGenerator : MonoBehaviour, IChip
{
    public ExpansionChipSlot expansionChipSlot { get; set; }
    private ExpansionChipStatus expansionChipStatus;
    public bool isBuffActive { get; set; }
    ExpansionChipStatus IChip.expansionChipStatus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();
    }
    void Update()
    {
        if(expansionChipSlot.isEquipped && !isBuffActive)
        {
            ApplyBuff();
        }
        else if(!expansionChipSlot.isEquipped && isBuffActive)
        {
            RemoveBuff();
        }
    }
    void ApplyBuff()
    {
        Debug.Log("Applying Buff: Energy Generator Chip");
        expansionChipStatus.isEnergyGeneratorEquipped = true;
        isBuffActive = true;
    }
    void RemoveBuff()
    {
        Debug.Log("Removing Buff: Energy Generator Chip");
        expansionChipStatus.isEnergyGeneratorEquipped = false;
        isBuffActive = false;
    }
}
