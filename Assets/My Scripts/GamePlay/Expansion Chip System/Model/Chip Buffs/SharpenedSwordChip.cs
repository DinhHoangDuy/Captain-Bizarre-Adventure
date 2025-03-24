using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpenedSwordChip : MonoBehaviour, IChip
{
    /*
        Increase 10 of Basic attack value when equipped
    */

    #region Dependencies
    public bool isBuffActive { get; set; }
    [HideInInspector] public ExpansionChipSlot expansionChipSlot {get; set;}

    private void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
    }
    #endregion
    private void Update()
    {
        if (expansionChipSlot.isEquipped && !isBuffActive)
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
        Debug.Log("Applying Buff: Sharpened Sword Chip");
        isBuffActive = true;
        ExpansionChipStatus.instance.isSharpenedSwordChipEquipped = true;
    }
    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Sharpened Sword Chip");
        isBuffActive = false;
        ExpansionChipStatus.instance.isSharpenedSwordChipEquipped = false;
    }    
}