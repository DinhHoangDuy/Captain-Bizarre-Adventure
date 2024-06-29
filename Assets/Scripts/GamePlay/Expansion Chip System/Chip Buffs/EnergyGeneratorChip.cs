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
    private ExpansionChipStatus expansionChipStatus;
    public bool isBuffActive { get; set; }
    ExpansionChipStatus IChip.expansionChipStatus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();
        // Set the max SP the chip can generate to the ExpansionChipStatus
        expansionChipStatus.SetEnergyGeneratorThresshold(maxSP);
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
        expansionChipStatus.isEnergyGeneratorEquipped = true;
        isBuffActive = true;
    }
    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Energy Generator Chip");
        expansionChipStatus.isEnergyGeneratorEquipped = false;
        isBuffActive = false;
    }
}
