using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event System.Action<float, DamageType> OnHitDamageReceived;
    public void TakeMeleeDamage(float damageAmount, DamageType DMGType)
    {
        OnHitDamageReceived?.Invoke(damageAmount, DMGType);
    }
}
