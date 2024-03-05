using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostDMG : MonoBehaviour
{
    //The total amount of damage boost (uses %)
    public int totalDMGBoost = 0;
    
    //Increase the totalDMGBoost by the amount of the boost
    public void IncreaseDMGBoost(int boost)
    {
        totalDMGBoost += boost;
    }
    //Decrease the totalDMGBoost by the amount of the boost
    public void DecreaseDMGBoost(int boost)
    {
        totalDMGBoost -= boost;
    }
    //Increase damage by the totalDMGBoost
    public float BoostDamage(float damage)
    {
        return damage + (damage * totalDMGBoost / 100);
    }

    public void Update()
    {
        if(totalDMGBoost < 0)
        {
            totalDMGBoost = 0;
        }
    }
}
