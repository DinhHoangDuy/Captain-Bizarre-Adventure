using UnityEngine;

public class TakeDMG : MonoBehaviour
{
    #region Enemy Take Damage
    public event System.Action<float, DamageType, DamageRange, DamageFromSkill> OnHitDamageReceived;
    public void TakeMeleeDamage(float damageAmount, DamageType DMGType, DamageFromSkill skill)
    {
        OnHitDamageReceived?.Invoke(damageAmount, DMGType, DamageRange.Melee, skill);
    }
    public void TakeRangeDamage(float damageAmount, DamageType DMGType, DamageFromSkill skill)
    {
        OnHitDamageReceived?.Invoke(damageAmount, DMGType, DamageRange.Range, skill);
    }
    #endregion

    //Destroyable Ojects take Damage
    public event System.Action<int> OnHitDestroyableReceived;
    public void TakeDestroyableDamage(int damageAmount)
    {
        OnHitDestroyableReceived?.Invoke(damageAmount);
    }

    #region Player Take Damage
    public event System.Action<int, Vector2> OnHitPlayerReceived;
    public void HitPlayer(int damageAmount, Vector2 damageSourcePosition)        
    {
        OnHitPlayerReceived?.Invoke(damageAmount, damageSourcePosition);
    }
    #endregion
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
public enum DamageFromSkill
{
    BasicAttack,
    UltimateSkill
}

public enum DamageOverTime
{
    Bleed,
    Burn
}