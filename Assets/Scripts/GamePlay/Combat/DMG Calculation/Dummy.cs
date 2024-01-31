using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(DMGInput))]
public class Dummy : MonoBehaviour
{
    private float DMGTaken = 0f;    
    private void Start()
    {
        // Get the Enemy script attached to the same GameObject
        DMGInput dmgInputScript = GetComponent<DMGInput>();
        if (dmgInputScript != null)
        {
            dmgInputScript.OnHitDamageReceived += HandleHitDamage;
        }
    }

    private void HandleHitDamage(float damageAmount, DamageType DMGType)
    {
        Debug.Log("DMG Character dealt " +  damageAmount + " " + DMGType + " damage");

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
*/