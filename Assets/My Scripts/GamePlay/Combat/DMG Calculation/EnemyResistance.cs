using System;
using UnityEngine;

[RequireComponent(typeof(EnemyNegativeStatus))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyResistance : MonoBehaviour
{
    #region Current Defensive Stats
    private float VunerableValue = 0;
    //VunerableValue variable will be used to calculate the vunerability of the enemy, the higher the value, the more damage the enemy will take
    // (1 point of vunerability = 1% more damage taken)
    #endregion

    private void Update()
    {
        VunerableValue = Mathf.Clamp(VunerableValue, 0, float.MaxValue); //Limit the VunerableValue as low as 0 and high as MaxValue
    }
    public void TakeDamage(float damageTaken)
    {
        HandleHitDamage(damageTaken);
    }

    private void HandleHitDamage(float damageTaken)
    {
        // Call some methods to calculate the damage taken
        // damageTaken = TypeResistanceCalculator(damageTaken);
        damageTaken = VunerabilityCalculator(damageTaken, VunerableValue);

        // Check if the GameObject has an EnemyHealth script attached to it, then call the TakeDamage method
        if (gameObject.GetComponent<EnemyHealth>() != null)
        {
            gameObject.GetComponent<EnemyHealth>().TakeDamage(damageTaken);
            Debug.Log("Enemy took " + damageTaken + " damage!");
        }
    }

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
