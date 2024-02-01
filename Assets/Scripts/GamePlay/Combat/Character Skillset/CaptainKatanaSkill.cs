using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CaptainKatanaSkill : MonoBehaviour
{
    //Grab status and numbers from PlayerStats script
    private PlayerStats playerStats;
    //Player Components
    private Animator CharacterAnimator;

    [Header("Captain's Ultimate")]
    [Tooltip("Captain Blood Moon Katana Ultimate Damage")][SerializeField] private float UltDMG = 70f;
    private float OriginalDMG;

    //Character Current Stats
    private bool ForceMovingForward = false;

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

    private void UltimateSkill()
    {
        Debug.Log("Captain's Ultimate Triggered!");
    }

    //These functions are created and added to the Animation Events
    //(Check them in the Animation Tab on the Editor)
    //These are meant to be used to control the animation and the damage and custom effects for each skill
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

    //Auto Update Animation State
    private void AutoUpdateAnimationState()
    {
        //CharacterAnimator.SetBool("Burst Mode", BurstModeState);
    }
}
