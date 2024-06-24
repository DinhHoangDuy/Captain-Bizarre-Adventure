using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenSwordChip : MonoBehaviour, IChip
{
    [Header("Debuff Value")]
    [Tooltip("Use % to determine the debuff value)")]
    public float debuffValue = 50f;

    #region Dependencies
    private bool isBuffActive = false;
    private ExpansionChipSlot expansionChipSlot;
    private DamageOutCalculator damageOutCalculator;

    private void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        damageOutCalculator = GameObject.FindGameObjectWithTag("Player").GetComponent<DamageOutCalculator>();
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
        Debug.Log("Applying Buff: Broken Sword Chip");
        damageOutCalculator.DecreaseDMGBoost(debuffValue);
        Debug.Log("Current DMG Boost: " + damageOutCalculator._totalDMGBoost);
        isBuffActive = true;
    }
    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Broken Sword Chip");
        damageOutCalculator.IncreaseDMGBoost(debuffValue);
        Debug.Log("Current DMG Boost: " + damageOutCalculator._totalDMGBoost);
        isBuffActive = false;
    }    
}