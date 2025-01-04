using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
    - This script will be used to handle the negative status that the enemy can receive from the player's skills
    - It will be automatically attached to the enemy GameObject which has the EnemyResistance script attached to it
    - The negative status will be applied to the enemy for a certain amount of time and will increase the damage taken by the enemy, or decrease the enemy's resistance to a certain type of damage, etc...
*/
public class EnemyNegativeStatus : MonoBehaviour
{
    #region Script Dependencies
    // Get the EnemyResistance script attached to the same GameObject
    private EnemyResistance enemyResistance;
    #endregion

    #region Enemy Unique Negative Status
    // Each negative status (let's call that debuff) will have a unique effect on the enemy. Each of them requires a different variable to store the value of the debuff, and a coroutine to handle the duration of the debuff.

    // This debuff will increase the damage taken by the enemy for a certain amount of time. This comes from the Samurai Captain's Ultimate Skill
    private bool isCursedByTheRules = false;
    private Coroutine CursedByTheRulesCoroutine;
    #endregion

    private void Awake()
    {
        enemyResistance = GetComponent<EnemyResistance>();
    }


    #region Cursed By The Rules (a unique negative effect from Captain's Ultimate Skill)
    private IEnumerator CursedByTheRules(int Vunerability, int seconds)
    {
        if(!isCursedByTheRules)
        {
            isCursedByTheRules = true;
            enemyResistance.IncreaseVunerableValue(Vunerability);
        }

        yield return new WaitForSeconds(seconds);
        isCursedByTheRules = false;
        enemyResistance.DecreaseVunerableValue(Vunerability);
    }
    public void ApplyCursedByTheRules(int Vunerability, int seconds)
    {
        if(CursedByTheRulesCoroutine != null)
        {
            StopCoroutine(CursedByTheRulesCoroutine);
        }
        CursedByTheRulesCoroutine = StartCoroutine(CursedByTheRules(Vunerability, seconds));
    }
    #endregion
}
