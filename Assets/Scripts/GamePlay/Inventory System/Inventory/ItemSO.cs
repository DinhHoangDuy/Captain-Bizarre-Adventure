using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string itemBackStory;
    [TextArea]
    public string itemDescription;
    public Sprite itemIcon;
    public StatsToChange statsToChange = new StatsToChange();
    [Tooltip("If it boost SP, use % instead")]
    public int amountToChange;

    public bool UseItem()
    {
        if (statsToChange == StatsToChange.Health)
        {
            var playerHealth = GameObject.Find("CaptainMoonBlade").GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogError("PlayerHealth component not found on CaptainMoonBlade");
                return false;
            }
            else
            {
                if(playerHealth.currentHealth == playerHealth._maxHealth)
                {
                    Debug.Log("Health is already full");
                    return false;
                }
                Debug.Log($"Increasing health by {amountToChange} health");
                playerHealth.IncreaseHealth(amountToChange);
                return true;
            }
        }

        if (statsToChange == StatsToChange.SP)
        {
            var playerSP = GameObject.Find("CaptainMoonBlade").GetComponent<PlayerSP>();
            if (playerSP == null)
            {
                Debug.LogError("PlayerSP component not found on CaptainMoonBlade");
                return false;
            }
            else
            {
                if(playerSP._currentSP == playerSP._maxSP)
                {
                    Debug.Log("SP is already full");
                    return false;
                }
                Debug.Log($"Increasing SP by {amountToChange}%");
                playerSP.IncreaseSP(amountToChange);
                return true;
            }
        }

        return false;
    }

    public enum StatsToChange
    {
        none,
        Health,
        SP,
    }
}
