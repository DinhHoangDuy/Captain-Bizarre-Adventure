using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(DMGInput))]
public class Dummy : MonoBehaviour
{ 
    private void Start()
    {
        // Get the Enemy script attached to the same GameObject
        DMGInput dmgInputScript = GetComponent<DMGInput>();
        if (dmgInputScript != null)
        {
            dmgInputScript.OnHitDamageReceived += HandleHitDamage;
            dmgInputScript.OnHitDestroyableReceived += HandleDestroyableDamage;
        }
    }

    private void HandleDestroyableDamage(int objDMG)
    {
        Debug.Log("As a destroyable object, Dummy took " + objDMG + " damage from the player");
    }

    private void HandleHitDamage(float damageAmount, DamageType DMGType, DamageRange damageRange)
    {
        Debug.Log("Character dealt " +  damageAmount + " " + DMGType + " of " + damageRange + " damage");
        Debug.Log("As an enemy, the dummy took " +  damageAmount + " " + DMGType + " of " + damageRange + " damage");
    }
}

/*Enemy Damage Resistance Template!
    switch (DMGType)
    {
        case DamageType.Fire:
            // Implement fire damage logic
            DMGTaken = damageAmount * 0.5f;
            break;
        case DamageType.Ice:
            // Implement ice damage logic
            DMGTaken = damageAmount * 1.5f;
            break;
        // Add more cases for other damage types as needed
        default:
            DMGTaken = damageAmount;
            break;
    }
    switch (damageRange)
    {
        case DamageRange.Melee:
            // Implement melee damage logic
            DMGTaken = damageAmount * 0.5f;  //This is to reduce the melee damage taken by 50%
            break;
        case DamageRange.Range:
            // Implement range damage logic
            DMGTaken = damageAmount * 1.5f; //This is to increase the range damage taken by 50%
            break;
        // Add more cases for other damage ranges as needed
        default:
            DMGTaken = damageAmount;
            break;
    }     
*/