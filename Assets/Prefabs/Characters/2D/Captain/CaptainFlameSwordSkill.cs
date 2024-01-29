using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class CaptainFlameSwordSkill : MonoBehaviour
{
    //Adapt the new Input system
    private PlatformerInputAction platformerInputaction;
    private InputAction fireInput;
    private InputAction ultInput;

    [Header("Captain's Attack Behaviors")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Captain's Stats")]
    [Tooltip("Captain Flame Sword Damage Type")][SerializeField] private DamageType damageType = DamageType.Fire;
    [Tooltip("Captain's Health")][SerializeField] private int health = 4;
    [Tooltip("Captain's Starter SP")][SerializeField] private float StartSP = 50f;
    [Tooltip("Captain's Maximum SP")][SerializeField] private float MaxSP = 50f;
    [Tooltip("Captain Flame Sword basic ATK damage")][SerializeField] private float BaseATKDamage = 30f;
    [Tooltip("Captain Flame Sword attack rate")][SerializeField] private float AttackRate = 2f;
    [Tooltip("Captain Flame Sword SP gain per attack")][SerializeField] private float SPGain = 1.2f;
    private float SPRegenBoost = 0f;
    private float currentATKDamage;
    float nextAttackTime = 0f;
    [Header("Captain's Ultimate")]
    [Tooltip("Captain Flame Sword Burst Damage Boost (use 1% to 100% format")][SerializeField] private float BurstDMGBuff = 30f;
    [Tooltip("Captain Flame Sword Burst Mode Duration")][SerializeField] private int BurstModeDuration = 15;
    [Tooltip("Captain's Required SP for the Ultimate")][SerializeField] private float RequiredSP = 40f;
    [Tooltip("Captain Flame Sword Ult Cooldown")][SerializeField] private int UltimateSkillCooldown = 23;


    //Debug Only
    private float currentSP;
    private bool BurstMode = false;
    private int BurstModeRemaining = 0;
    private int UltCurrentCooldown = 0;

    private Animator CharacterAnimator;
    private void OnEnable()
    {
        //Enable Moving
        fireInput = platformerInputaction.Player.Fire;
        fireInput.performed += BasicAttack;
        fireInput.Enable();

        ultInput = platformerInputaction.Player.Ultimate;
        ultInput.performed += Ultimate;
        ultInput.Enable();
    }

    private void OnDisable()
    {
        fireInput.Disable();
        ultInput.Disable();
    }

    private void Awake()
    {
        platformerInputaction = new PlatformerInputAction();
        CharacterAnimator = GetComponent<Animator>();
    }
    void Start()
    {        
        currentATKDamage = BaseATKDamage;
        currentSP = StartSP;
    }

    //If the fire and ultimate button is pressed (performed)
    private void BasicAttack(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;
        if (Time.time >= nextAttackTime)
        {
            if (PlatformerMovement2D.isGrounded)
            {
                CharacterAnimator.SetTrigger("Basic Attack");
                nextAttackTime = Time.time + 1f / AttackRate;
            }
        }
    }
    private void Ultimate(InputAction.CallbackContext context)
    {
        if (!BurstMode)
        {
            if ((currentSP >= RequiredSP) && UltCurrentCooldown == 0)
            {
                currentSP -= RequiredSP;
                StartCoroutine(UltimateSkill());
            }
            else if (currentSP < RequiredSP)
            {
                Debug.Log("Not enough SP!");
            }
            else if (UltCurrentCooldown > 0)
            {
                Debug.Log("The Ult is not ready!");
            }
        }
        else Debug.Log("Burst Mode is Active! You can't use Ultimate during Burst Mode");
    }
    private IEnumerator UltimateSkill()
    {
        //Enter Burst mode and change Character Visual
        currentATKDamage += BaseATKDamage * ((100 + BurstDMGBuff) / 100);
        Debug.Log("Burst Mode Started, " + BurstModeDuration + " remaning!");
        //Update character behavior and put ultimate on a cooldown
        StartCoroutine(UltCooldownCoroutine(UltimateSkillCooldown));
        CharacterAnimator.SetBool("Burst Mode", true);
        BurstMode = true;
        //This Coroutine is meant to check duration only
        StartCoroutine(TrackBurstModeDuration(BurstModeDuration));

        //Wait until the Burst mode ends
        yield return new WaitForSeconds(BurstModeDuration);

        //Exit Burst mode and change Character Visual
        CharacterAnimator.SetBool("Burst Mode", false);
        BurstMode = false;
        currentATKDamage = BaseATKDamage;
        Debug.Log("Burst Mode Ended");
    }
    private IEnumerator UltCooldownCoroutine(int UltimateSkillCooldown)
    {
        UltCurrentCooldown = UltimateSkillCooldown;

        while (UltCurrentCooldown > 0)
        {
            //ultimateButton.interactable = false; // Disable button

            yield return new WaitForSeconds(1f); // Wait for 1 second
            UltCurrentCooldown--;
        }
        //Activate this part when the ult is ready!
        //ultimateButton.interactable = true; // Enable button after cooldown
    }

    //This function is created and added to the Animation Events (Check them in the Animation Tab on the Editor)
    public void BasicATKEnemy()
    {
        //Detect enemy and obstacles (both of them all listed in the LayerMask[] enemyLayerMask
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayerMask);
        //Check if Captain hits any enemy/dummy
        if (hitEnemy != null)
        {
            //Gain SP if Captain hits enemy (it can stack if Captain hit multiple enemy in one hit, but it only boost a little more, and it's indepent from SPRegenBoost)
            if (currentSP < MaxSP)
            {
                if (hitEnemy.Length > 1)
                {
                    currentSP += SPGain * (1 + SPRegenBoost) + 0.5f;
                }
                else currentSP += SPGain * (1 + SPRegenBoost);
            }
            //Damage enemy
            foreach (Collider2D enemy in hitEnemy)
            {
                enemy.GetComponent<Enemy>().TakeMeleeDamage(currentATKDamage, damageType);
                //enemy.GetComponent<Enemy>().TakeMeleeDamage(currentATKDamage);
            }
        }
    }

    //This section works in Unity Editor and will not affect gameplay
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    private IEnumerator TrackBurstModeDuration(int BurstModeDuration)
    {
        BurstModeRemaining = BurstModeDuration;
        while (BurstModeRemaining > 0)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second
            BurstModeRemaining--;
        }
    }
}

    