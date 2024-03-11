using System;
using UnityEngine;

[RequireComponent(typeof(TakeDMG))]
public class EnemyResistance : MonoBehaviour
{
    #region Enemy Resistance
    [Header("Enemy Resistance to Elemental Damage")]
    [SerializeField] private int fireResistance;
    [SerializeField] private int iceResistance;
    [SerializeField] private int lightningResistance;
    [SerializeField] private int physicalResistance;
    [Header("Enemy Resistance to Damage Range")]
    [SerializeField] private int meleeResistance;
    [SerializeField] private int rangeResistance;

    private TakeDMG TakeDMGScript;
    #endregion
    
    private void Awake()
    {
        // Get the Enemy script attached to the same GameObject
        TakeDMGScript = GetComponent<TakeDMG>();
        if (TakeDMGScript != null)
        {
            TakeDMGScript.OnHitDamageReceived += HandleHitDamage;
        }
    }

    private void HandleHitDamage(float damageTaken, DamageType DMGType, DamageRange damageRange)
    {
        // Call the ResistanceCalculator method to calculate the damage taken, then pass the result to the EnemyHealth/Dummy
        damageTaken = ResistanceCalculator(damageTaken, DMGType, damageRange);
        // Check if the GameObject has an EnemyHealth or Dummy script attached to it, then call the TakeDamage method
        if(gameObject.GetComponent<EnemyHealth>() != null)
        {
            gameObject.GetComponent<EnemyHealth>().TakeDamage(damageTaken);
        }
        else if(gameObject.GetComponent<Dummy>() != null)
        {
            gameObject.GetComponent<Dummy>().TakeDamage(damageTaken);
        }
    }
    private float ResistanceCalculator(float damageTaken, DamageType DMGType, DamageRange damageRange)
    {
        switch (DMGType)
        {
            case DamageType.Fire:
                damageTaken = damageTaken * (1 - (fireResistance / 100));
                break;
            case DamageType.Ice:
                damageTaken = damageTaken * (1 - (iceResistance / 100));
                break;
            case DamageType.Lightning:
                damageTaken = damageTaken * (1 - (lightningResistance / 100));
                break;
            case DamageType.Physical:
                damageTaken = damageTaken * (1 - (physicalResistance / 100));
                break;
        }
        switch (damageRange)
        {
            case DamageRange.Melee:
                damageTaken = damageTaken * (1 - (meleeResistance / 100));
                break;
            case DamageRange.Range:
                damageTaken = damageTaken * (1 - (rangeResistance / 100));
                break;
        }  
        return damageTaken;
    }
}
