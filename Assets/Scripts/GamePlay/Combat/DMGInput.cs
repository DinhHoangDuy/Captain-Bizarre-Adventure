using UnityEngine;

public class DMGInput : MonoBehaviour
{
    public event System.Action<float, DamageType> OnHitDamageReceived;
    public void TakeMeleeDamage(float damageAmount, DamageType DMGType)
    {
        OnHitDamageReceived?.Invoke(damageAmount, DMGType);
    }
    public void TakeRangeDamage(float damageAmount, DamageType DMGType)
    {
        OnHitDamageReceived?.Invoke(damageAmount, DMGType);
    }
}
