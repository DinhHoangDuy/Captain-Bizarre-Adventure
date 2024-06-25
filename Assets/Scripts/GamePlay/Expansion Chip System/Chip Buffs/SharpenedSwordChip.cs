using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpenedSwordChip : MonoBehaviour, IChip
{
    [Header("Buff Value")]
    [Tooltip("Use % to determine the buff value)")]
    public float buffValue = 5f;

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
        damageOutCalculator.IncreaseDMGBoost(buffValue);
        Debug.Log("Current DMG Boost: " + damageOutCalculator._totalDMGBoost);
        isBuffActive = true;
    }
    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Sharpened Sword Chip");
        damageOutCalculator.DecreaseDMGBoost(buffValue);
        Debug.Log("Current DMG Boost: " + damageOutCalculator._totalDMGBoost);
        isBuffActive = false;
    }    
}