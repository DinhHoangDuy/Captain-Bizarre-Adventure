using System;
using UnityEngine;

[RequireComponent(typeof(TakeDMG))]
[RequireComponent(typeof(EnemyNegativeStatus))]
public class EnemyResistance : MonoBehaviour
{
    #region Enemy Resistance
    [Tooltip("You can set the resistance to a maximum of 100% to make the enemy immune to that type of damage, or set it to a negative value (up to -100%) to make the enemy vunerable to that type of damage.")]
    [Header("Enemy Resistance to Elemental Damage")]
    [SerializeField] private int fireResistance;
    [SerializeField] private int iceResistance;
    [SerializeField] private int lightningResistance;
    [SerializeField] private int physicalResistance;

    [Tooltip("You can set the resistance to a maximum of 100% to make the enemy immune to that type of damage, or set it to a negative value (up to -100%) to make the enemy vunerable to that type of damage.")]
    [Header("Enemy Resistance to Damage Range")]
    [SerializeField] private int meleeResistance;
    [SerializeField] private int rangeResistance;

    [Tooltip("You can set the resistance to a maximum of 100% to make the enemy immune to that type of damage, or set it to a negative value (up to -100%) to make the enemy vunerable to that type of damage.")]
    [Header("Enemy Resistance to Character Skills")]
    [SerializeField] private int basicAttackResistance;
    [SerializeField] private int ultimateSkillResistance;
    #endregion
    private TakeDMG TakeDMGScript;

    #region Current Defensive Stats
    private float VunerableValue = 0; 
    //VunerableValue variable will be used to calculate the vunerability of the enemy, the higher the value, the more damage the enemy will take
    // (1 point of vunerability = 1% more damage taken)
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
    private void Update()
    {
        VunerableValue = Mathf.Clamp(VunerableValue, 0, float.MaxValue); //Limit the VunerableValue as low as 0 and high as MaxValue
    }

    private void HandleHitDamage(float damageTaken, DamageType DMGType, DamageRange damageRange, DamageFromSkill skill)
    {
        // Call some methods to calculate the damage taken
        damageTaken = TypeResistanceCalculator(damageTaken, DMGType, damageRange, skill);
        damageTaken = VunerabilityCalculator(damageTaken, VunerableValue);

        // Check if the GameObject has an EnemyHealth or Dummy script attached to it, then call the TakeDamage method
        if(gameObject.GetComponent<EnemyHealth>() != null)
        {
            gameObject.GetComponent<EnemyHealth>().TakeDamage(damageTaken);
            Debug.Log("Enemy took " + damageTaken + " damage!");
        }
        else if(gameObject.GetComponent<Dummy>() != null)
        {
            gameObject.GetComponent<Dummy>().TakeDamage(damageTaken);
            Debug.Log("Dummy took " + damageTaken + " damage!");
        }
    }

    #region Resistance Calculator
    private float TypeResistanceCalculator(float damageTaken, DamageType DMGType, DamageRange damageRange, DamageFromSkill skill)
    {
        //This part will calculate the damage taken based on the Elemental and Damage Range Resistance of the enemy
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
        switch (skill)
        {
            case DamageFromSkill.BasicAttack:
                damageTaken = damageTaken * (1 - (basicAttackResistance / 100));
                break;
            case DamageFromSkill.UltimateSkill:
                damageTaken = damageTaken * (1 - (ultimateSkillResistance / 100));
                break;
        }
        return damageTaken;
    }
    #endregion

    #region Vunerability Calculator
    private float VunerabilityCalculator(float damageTaken, float vunerableValue)
    {
        damageTaken = damageTaken * (1 + (vunerableValue / 100));
        return damageTaken;
    }
    public void IncreaseVunerableValue(float value)
    {
        VunerableValue += value;
    }
    public void DecreaseVunerableValue(float value)
    {
        VunerableValue -= value;
    }
    #endregion
}
