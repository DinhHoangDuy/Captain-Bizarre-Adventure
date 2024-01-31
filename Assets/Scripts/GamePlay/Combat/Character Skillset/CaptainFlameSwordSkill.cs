using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlatformerMovement2D))]
[RequireComponent(typeof(PlayerStats))]
public class CaptainFlameSwordSkill : MonoBehaviour
{

    //Grab status and numbers from PlayerStats script
    private PlayerStats playerStats;
    //Player Components
    private Animator CharacterAnimator;
    private PlatformerMovement2D platformerMovement2D;

    [Header("Captain's Ultimate")]
    [Tooltip("Captain Flame Sword Burst Damage Boost (use 1% to 100% format")][SerializeField] private float BurstDMGBuff = 30f;
    [Tooltip("Captain Flame Sword Burst Mode Duration")][SerializeField] private int BurstModeDuration = 15;
    private float OriginalDMG;

    //Character Current Stats
    private bool BurstModeState = false;

    private void OnEnable()
    {
        PlayerStats.OnUltimateTriggered += UltimateSkill;
    }
    private void OnDisable()
    {
        PlayerStats.OnUltimateTriggered -= UltimateSkill;
    }

    private void Awake()
    {
        CharacterAnimator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
    }
    private void Start()
    {
        OriginalDMG = playerStats.currentATKDamage;
    }
    private void FixedUpdate()
    {
        AutoUpdateAnimationState();
    }

    //This function is created and added to the Animation Events (Check them in the Animation Tab on the Editor)
    public void BlockAllMovement()
    {
        playerStats.ATKAllowed = false;

        PlatformerMovement2D.blocked = true;
    }
    public void UnblockAllMovement()
    {
        playerStats.ATKAllowed = true;
        PlatformerMovement2D.blocked = false;
    }
    public void BasicATKEnemy()
    {
        //Detect enemy and obstacles (both of them all listed in the LayerMask[] enemyLayerMask
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(playerStats.attackPoint.position, playerStats.attackRange, playerStats.enemyLayerMask);
        //Check if Captain hits any enemy/dummy
        if (hitEnemy != null)
        {
            if (playerStats.currentSP < playerStats.MaxSP)
            {
                if (hitEnemy.Length > 1)
                {
                    playerStats.currentSP += playerStats.SPGain * (1 + playerStats.SPRegenBoost) + 0.5f;
                }
                else playerStats.currentSP += playerStats.SPGain * (1 + playerStats.SPRegenBoost);
            }
            //Damage enemy
            foreach (Collider2D enemy in hitEnemy)
            {
                enemy.GetComponent<DMGInput>().TakeMeleeDamage(playerStats.currentATKDamage, playerStats.damageType);
            }
        }
    }
    private void UltimateSkill()
    {
        Debug.Log("Captain's Ultimate Triggered!");
        StartCoroutine(BurstMode(BurstModeDuration));
    }
    private IEnumerator BurstMode(int BurstModeDuration)
    {
        BurstModeState = true;
        playerStats.currentATKDamage += playerStats.currentATKDamage * BurstDMGBuff / 100;
        yield return new WaitForSeconds(BurstModeDuration);
        BurstModeState = false;
        playerStats.currentATKDamage = OriginalDMG;
    }

    //Auto Update Animation State
    private void AutoUpdateAnimationState()
    {
        CharacterAnimator.SetBool("Burst Mode", BurstModeState);
    }
}

