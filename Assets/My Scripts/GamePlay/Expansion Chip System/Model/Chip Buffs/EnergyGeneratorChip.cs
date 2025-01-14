using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGeneratorChip : MonoBehaviour, IChip
{
    /*
        When equipped, if the character has less than 60 SP, gain 1 SP per second
    */
    [Header("Max SP the chip can generate")]
    public int maxSP = 60;
    public ExpansionChipSlot expansionChipSlot { get; set; }
    public bool isBuffActive { get; set; }

    void Awake()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
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
    public void ApplyBuff()
    {
        Debug.Log("Applying Buff: Energy Generator Chip");
        ExpansionChipStatus.instance.isEnergyGeneratorEquipped = true;
        isBuffActive = true;
    }
    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Energy Generator Chip");
        ExpansionChipStatus.instance.isEnergyGeneratorEquipped = false;
        isBuffActive = false;
    }
}
