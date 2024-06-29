using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpenedSwordChip : MonoBehaviour, IChip
{
    /*
        Increase 5% of Basic attack value when equipped
    */
    [Header("Buff Value")]
    [Tooltip("Use % to determine the buff value)")]
    public float buffValue = 5f;
    private float originalDamage;

    #region Dependencies
    public bool isBuffActive { get; set; }
    [HideInInspector] public ExpansionChipSlot expansionChipSlot {get; set;}
    public ExpansionChipStatus expansionChipStatus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    private CaptainMoonBlade skillset;

    private void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        skillset = GameObject.FindGameObjectWithTag("Player").GetComponent<CaptainMoonBlade>();
        originalDamage = skillset.basicATK;
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
        skillset.basicATK += originalDamage * (buffValue / 100);
        isBuffActive = true;
    }
    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Sharpened Sword Chip");
        skillset.basicATK = originalDamage;
        isBuffActive = false;
    }    
}