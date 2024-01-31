using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    [Header("Character's Attack Behaviors")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayerMask;

    [Header("Character's Stats")]
    [Tooltip("Character 's Health")] public int health;
    [Tooltip("Damage Type")] public DamageType damageType = DamageType.Fire;
    [Tooltip("Character 's Starter SP")] public float StartSP;
    [Tooltip("Character 's Maximum SP")] public float MaxSP;
    [Tooltip("Character basic ATK damage")] public float BaseATKDamage = 30f;
    [Tooltip("Character SP gain per attack")] public float SPGain = 1.2f;

    [Space(10)]

    [Header("Character's Ultimate")]
    [Tooltip("Required SP for the Ultimate")] public float ultRequiredSP = 40f;
    [Tooltip("Ultimate Cooldown")] public int UltimateSkillCooldown = 23;

    [Space(10)]
    //Player Components
    private Animator CharacterAnimator;
    private PlatformerMovement2D platformerMovement2D;

    //Player current stats (this is meant to be used for the Character skill scripts)
    [HideInInspector] public float SPRegenBoost = 0f;
    [HideInInspector] public float currentATKDamage;
    [HideInInspector] public float currentSP;

    //Only use these variables in this script
    private int UltCurrentCooldown = 0;
    //Character Current Stats
    public bool ATKAllowed = true;

    //Adapt the new Input system
    private PlatformerInputAction platformerInputAction;
    private InputAction fireInput;
    private InputAction ultInput;
    //Create Events for the Ultimate Skill (this is meant to be used for the Character skill scripts)
    public static event Action OnUltimateTriggered;

    //This is for Debugging only
    private bool maxSPLogged = false;
    private bool noSPLogged = false;
    private bool ultReadyLogged = false;
    private void OnEnable()
    {
        //Enable Moving
        fireInput = platformerInputAction.Player.Fire;
        fireInput.performed += BasicAttack;
        fireInput.Enable();

        ultInput = platformerInputAction.Player.Ultimate;
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
        platformerInputAction = new PlatformerInputAction();
        CharacterAnimator = GetComponent<Animator>();
        platformerMovement2D = GetComponent<PlatformerMovement2D>();
    }
    private void Start()
    {
        currentATKDamage = BaseATKDamage;
        currentSP = StartSP;
    }
    private void FixedUpdate()
    {
        if (currentSP > MaxSP) currentSP = MaxSP;
        TrackCurrentSP();
        UpdateUltCooldown();
    }
    //If the fire and ultimate button is pressed (performed)
    private void BasicAttack(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;
        if (!ATKAllowed)
        {
            Debug.Log("Basic ATK failed: Disallowed!");
            return;
        }
        if (platformerMovement2D.UpdateIsGrounded())
        {
            CharacterAnimator.SetTrigger("Basic Attack");
        }
    }
    //Control the Ultimate Skill behavior (this is meant to be used WITH the Character skill scripts)
    #region Ultimate Skill
    private void Ultimate(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;
        if (currentSP >= ultRequiredSP)
        {
            if (UltReady())
            {
                currentSP -= ultRequiredSP;
                OnUltimateTriggered?.Invoke();
                StartCoroutine(StartUltCooldown(UltimateSkillCooldown));
            }
            else
            {
                Debug.Log("The Ult is not ready!");
            }
        }
        else
        {
            Debug.Log("Not enough SP!");
        }
    }
    private void UpdateUltCooldown()
    {
        UltReady();
    }
    private IEnumerator StartUltCooldown(int ultimateSkillCooldown)
    {
        UltCurrentCooldown = ultimateSkillCooldown;
        while (UltCurrentCooldown > 0)
        {
            UltCurrentCooldown--;
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
    }
    private bool UltReady()
    {
        //If the cooldown is 0, return true. Else, return false
        if (UltCurrentCooldown == 0)
        {
            return true;
        }
        else return false;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion
    //Debugging Only
    private void TrackCurrentSP()
    {
        if (currentSP == MaxSP)
        {
            if (!maxSPLogged)
            {
                Debug.Log("Max SP!");
                maxSPLogged = true;
            }
        }
        else
        {
            maxSPLogged = false;
        }

        if (currentSP == 0)
        {
            if (!noSPLogged)
            {
                Debug.Log("No SP!");
                noSPLogged = true;
            }
        }
        else
        {
            noSPLogged = false;
        }

        if (currentSP >= ultRequiredSP && UltReady())
        {
            if (!ultReadyLogged)
            {
                Debug.Log("Ult is ready!");
                ultReadyLogged = true;
            }
        }
        else
        {
            ultReadyLogged = false;
        }
    }
}
