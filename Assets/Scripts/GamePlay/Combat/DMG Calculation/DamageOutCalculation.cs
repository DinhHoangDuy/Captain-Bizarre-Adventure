using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOutCalculator : MonoBehaviour
{
    //The total amount of damage boost (uses %)
    private int totalDMGBoost = 0;
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
        Mathf.Clamp(totalDMGBoost, int.MinValue, int.MaxValue);
    }
}

