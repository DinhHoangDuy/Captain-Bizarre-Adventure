using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //This script is attached to the health bar Game Object containing 2 images, one for the background and one for the fill
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    [SerializeField] private PlayerHealth playerHealth; //Reference to the player's health script

    private void Start()
    {
        //Defensive programming to make sure the health bar is not null
        if(totalHealthBar == null)
        {
            Debug.LogError("Total Health Bar is null!!");
            return;
        }
        if(currentHealthBar == null)
        {
            Debug.LogError("Current Health Bar is null!!");
            return;
        }
        //Get the player's health script to access the health value, and set the max health value of the health bar
        totalHealthBar.fillAmount = (float)(playerHealth.maxHealth / 10.0);
    }
    private void Update()
    {
        //Set the current health value of the health bar
        currentHealthBar.fillAmount = (float)(playerHealth.currentHealth / 10.0);
    }
}