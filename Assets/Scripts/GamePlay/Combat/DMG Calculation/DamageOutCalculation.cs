using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOutCalculator : MonoBehaviour
{
    //The total amount of damage boost (uses %)
    [SerializeField] private int totalDMGBoost = 0;
    public int _totalDMGBoost { get { return totalDMGBoost; } }
    
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

    private void Update()
    {
        if(totalDMGBoost < 0)
        {
            totalDMGBoost = 0;
        }

        //Limit the totalDMGBoost as low as 0% and high as MaxValue
        Mathf.Clamp(totalDMGBoost, 0, int.MaxValue);
    }
}

