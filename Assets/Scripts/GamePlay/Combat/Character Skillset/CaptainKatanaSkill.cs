using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaptainKatanaSkill : MonoBehaviour
{
    //Adapt the new Input system
    private PlatformerInputAction platformerInputaction;
    private InputAction fireInput;
    private InputAction ultInput;
    //Grab status from PlatformerMovement2D script
    private PlatformerMovement2D platformerMovement2D;
    //Player Components
    private Animator CharacterAnimator;
    [Header("Character's Attack Behaviors")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayerMask;
    private float attackRange = 1f;

    [Header("Captain's Stats")]
    [Tooltip("Damage Type")][SerializeField] private DamageType damageType = DamageType.Fire;
    [Tooltip("Maximum Health")][SerializeField] private int health = 4;
    [Tooltip("Starter SP")][SerializeField] private float StartSP = 50f;
    [Tooltip("Maximum SP")][SerializeField] private float MaxSP = 50f;
    [Tooltip("Basic ATK Damage")][SerializeField] private float BaseATKDamage = 30f;
    [Tooltip("Attack rate")][SerializeField] private float AttackRate = 2f;
    [Tooltip("SP gain per attack")][SerializeField] private float SPGain = 1.2f;
    private float SPRegenBoost = 0f;
    private float currentATKDamage;
    float nextAttackTime = 0f;

    [Header("Captain's Ultimate")]
    [Tooltip("Required SP for the Ultimate")][SerializeField] private float RequiredSP = 40f;
    [Tooltip("Ultimate Cooldown")][SerializeField] private int UltimateSkillCooldown = 23;

    //Player current stats
    private float currentSP;
    private int UltCurrentCooldown = 0;
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
            if (platformerMovement2D.UpdateIsGrounded())
            {
                CharacterAnimator.SetTrigger("Basic Attack");
                nextAttackTime = Time.time + 1f / AttackRate;
                Debug.Log("Basic Attack");
            }
        }
    }
    private void Ultimate(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;
        if (currentSP >= RequiredSP)
        {
            CharacterAnimator.SetTrigger("Ultimate");
            currentSP -= RequiredSP;
            Debug.Log("Ultimate");
        }
        else Debug.Log("Not enough SP");
    }
}
