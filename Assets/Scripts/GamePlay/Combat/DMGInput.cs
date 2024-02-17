using UnityEngine;

public class DMGInput : MonoBehaviour
{
    //Take Damage
    public event System.Action<float, DamageType, DamageRange> OnHitDamageReceived;
    public void TakeMeleeDamage(float damageAmount, DamageType DMGType)
    {
        OnHitDamageReceived?.Invoke(damageAmount, DMGType, DamageRange.Melee);
    }
    public void TakeRangeDamage(float damageAmount, DamageType DMGType)
    {
        OnHitDamageReceived?.Invoke(damageAmount, DMGType, DamageRange.Range);
    }

    //Destroyable Ojects take Damage
    public event System.Action<int> OnHitDestroyableReceived;
    public void TakeDestroyableDamage(int damageAmount)
    {
        OnHitDestroyableReceived?.Invoke(damageAmount);
    }
}

public enum DamageRange
{
    Melee,
    Range
}
public enum DamageType
{
    Physical,
    Fire,
    Ice,
    Lightning
}


public enum DamageOverTime
{
    Bleed,
    Burn
}