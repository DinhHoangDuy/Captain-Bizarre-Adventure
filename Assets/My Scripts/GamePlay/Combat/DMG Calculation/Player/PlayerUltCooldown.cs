using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUltCooldown : MonoBehaviour
{
    private UltimateIcon ultimateIcon;
    private CaptainSkillSet skillset;
    private float currentUltimateCooldown;
    private float ultCooldown;
    private bool isUltimateReady = false;

    private void Awake()
    {
        skillset = GetComponent<CaptainSkillSet>();
        ultimateIcon = FindObjectOfType<UltimateIcon>();
    }

    private void Start()
    {
        ultCooldown = skillset._ultimateCooldown;
    }
    private void Update()
    {
        isUltimateReady = skillset.CanCastUltimate();
        if(isUltimateReady)
        {
            ultimateIcon.DisplayUltimateReady();
        }
        else
        {
            ultimateIcon.DisplayUltimateNotReady();
        }

        // Display the cooldown of the ultimate skill
        currentUltimateCooldown = CaptainSkillSet.currentUltimateCooldown;
        if (ultimateIcon != null)
        {
            ultimateIcon.UpdateCooldownTime(currentUltimateCooldown, ultCooldown);
        }
        else
        {
            Debug.LogError("ultimateIcon is not assigned");
        }
    }
}
