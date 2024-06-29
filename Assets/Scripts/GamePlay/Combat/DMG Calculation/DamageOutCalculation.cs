using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOutCalculator : MonoBehaviour
{
    public static DamageOutCalculator instance;
    //The total amount of damage boost (uses %)
    private float totalDMGBoost = 0;
    public float _totalDMGBoost { get { return totalDMGBoost; } }
    
    //Increase the totalDMGBoost by the amount of the boost
    public void IncreaseDMGBoost(float boost)
    {
        totalDMGBoost += boost;
    }
    //Decrease the totalDMGBoost by the amount of the boost
    public void DecreaseDMGBoost(float boost)
    {
        totalDMGBoost -= boost;
    }
    //Increase damage by the totalDMGBoost
    public float BoostDamage(float damage)
    {
        return damage + (damage * totalDMGBoost / 100);
    }
    public float MacabreDanceTotalDMGBoost(float damage)
    {
        return damage + (damage * (totalDMGBoost + ExpansionChipStatus.instance.MacabreDanceTotalDMGBoost) / 100);
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        Mathf.Clamp(totalDMGBoost, int.MinValue, int.MaxValue);
    }
}

