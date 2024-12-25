using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenSwordChip : MonoBehaviour, IChip
{
    [Header("Debuff Value")]
    [Tooltip("Use % to determine the debuff value)")]
    public float debuffValue = 50f;

    #region Dependencies
    public bool isBuffActive { get; set; }
    [HideInInspector] public ExpansionChipSlot expansionChipSlot {get; set;}
    public ExpansionChipStatus expansionChipStatus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private DamageOutCalculator damageOutCalculator;

    private void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        damageOutCalculator = GameObject.FindGameObjectWithTag("Player").GetComponent<DamageOutCalculator>();
    }
    #endregion
    private void Update()
    {
        // Check if the chip is equipped and the buff is not active
        if (expansionChipSlot.isEquipped && !isBuffActive)
        {
            ApplyBuff();
        }
        // Check if the chip is not equipped and the buff is active
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