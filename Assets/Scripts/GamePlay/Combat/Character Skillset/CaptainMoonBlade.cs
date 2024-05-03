using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DamageOutCalculator))]
[RequireComponent(typeof(PlatformerMovement2D))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(CaptainMoonBladeAnimation))]
[RequireComponent(typeof(ExpansionChips))]
public class CaptainMoonBlade : MonoBehaviour
{
    #region Captain's Skill Set Description
    /*
    Captain's Skill Set Description:
    - Basic Damage: 100
    - Critical Rate: 20%
    - Critical Damage: 150%
    - Health: 5 hearts stacks
    - Damage type: Physical
    - Blood Moon Blade: Captain's basic attack is a 2-hit in a single button. Each hit deals 10 + 60%/70%/80% of Captain's basic attack damage as physical damage.
    - About the Ultimate skill "The Vow under the Moon":
        + Shoots a wave of energy in a straight line, dealing 150/170 (+ 105% of physical attack) as physical damage to the first enemy hit. This attack can't crit.
        + "The Vow under the Moon" will grant the "Unbreakable Will" for 5 seconds if the buff is not active.
    - About the Ultimate skill requirement:
        + The skill will use a stack system.
            * The skill will consume 1 stack and 50 SP to cast.
    - Passive "Unbreakable Will": 
        + Each Basic Attack has a 20%/30%/40% chance to grant "Unbreakable Will" for 5 seconds. This effect will be refreshed if the effect is triggered again during the duration.
        + When "Unbreakable Will" is active, Captain will gain 30% total Damage Boost.
        + "The Vow under the Moon" will deal 10% more damage if "Unbreakable Will" is active. This Ultimate will be consumed if the wave hits an enemy. 
    */
    #endregion

    #region Captain's Skill Set Attributes
        [Header("Captain's Skill Set Attributes")]
        [Header("Captain's Basic Attributes")]
            [SerializeField] private float basicDamage = 100;
            [SerializeField] private DamageType damageType = DamageType.Physical;
            [SerializeField] private float criticalRate = 20f;
            [SerializeField] private float criticalDamageMultiplier = 150f;
            [SerializeField] private int startSP = 15;
            public int _startSP { get { return startSP; } }
            [SerializeField] private int SPRegenRate = 1;
            [SerializeField] private int SPRegenEfficiency = 100;
            [SerializeField] private int maxSP = 70;
            public int _maxSP { get { return maxSP; } }

        [Header("Captain's Basic Attack Attributes")]
            [SerializeField] private int basicAttackBaseDMG = 10;
            [SerializeField] private float basicAttackMultiplier = 60f;
            [SerializeField] private float basicAttackUWTriggerChance = 20f;

        [Header("Captain's Ultimate Attributes")]
            [SerializeField] private GameObject waveOfEnergyPrefab;
            [SerializeField] private float ultimateBaseDamage = 150;
            [SerializeField] private float ultimateDamageMultiplier = 105f;

            [Tooltip("Ultimate Boost when passive is available")]
            [SerializeField] private float ultimateUWPassiveBonus = 10f;

            [SerializeField] private float ultimateCooldown = 10f;
            [SerializeField] private int requiredSP = 30;

        
        //Passive
        [Header("Captain's Passive Attributes")]
            [SerializeField] private float passiveDuration = 5f;
        [SerializeField] private int passiveDMGBoost = 30;
       
    #endregion

    #region Script Dependencies
        [Header("Script Dependencies")]
        private PlatformerMovement2D platformerMovement2D;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private LayerMask destroyableLayers;
        private DamageOutCalculator dmgCalulator;
        private ExpansionChips expansionChips;
    #endregion

    #region Current Status 
    [SerializeField] private bool isPassiveActive = false;
    private float basicAttackDamage;
    public bool fireTriggered;
    private bool criticalHit = false;
    private int currentSP;
    public int _currentSP { get { return currentSP; } }

    public bool ultimateTriggered;
    private float ultimateDamage;
    private float currentUltimateCooldown = 0f;
    public float _currentUltimateCooldown { get { return currentUltimateCooldown; } }
    public float _ultimateCooldown { get { return ultimateCooldown; } }
    #endregion   

    #region New Input System
    private PlayerInput playerInput;
    private InputAction FireInput;
    private InputAction UltimateInput;
    

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        FireInput = playerInput.Game.Fire;
        UltimateInput = playerInput.Game.Ultimate;
        UltimateInput.performed += ctx => UltimatePressed();
        playerInput.Enable();
    }
    private void UltimatePressed()
    {
        UltimateAttack();
    }

    #endregion

    private void Awake()
    {
        platformerMovement2D = GetComponent<PlatformerMovement2D>();
        dmgCalulator = GetComponent<DamageOutCalculator>();
        expansionChips = GetComponent<ExpansionChips>();
    }
    private void Start()    
    {
        // Set the current SP to the start SP
        currentSP = startSP;
        
        //Get a Warning if the wave of energy prefab is not assigned
        if(waveOfEnergyPrefab == null)
        {
            Debug.LogWarning("Wave of Energy Prefab is not assigned to the Captain's Skill Set");
        }

        // Implement the Expansion Chips
        if(expansionChips._SharpBlade)
        {
            dmgCalulator.IncreaseDMGBoost(expansionChips._SharpBladeDMGBoost);
        }
        if(expansionChips._WeakBody)
        {
            dmgCalulator.DecreaseDMGBoost(50);
        }
    }
    private void Update()
    {
        // Increase SP per 4 seconds if the Sweet Snacks is equiped
        if(expansionChips._SweetSnacks)
        {
            if(sweetSnacksCoroutine == null)
            {
                sweetSnacksCoroutine = StartCoroutine(SweetSnacksSPRegen(expansionChips._SweetSnacksSPRegenRate, expansionChips._SweetSnacksSPRegenAmount));
            }
        }

        // Limit the SP to the maxSP
        currentSP = Mathf.Clamp(currentSP, 0, maxSP);

        // Limite the SP regen efficiency as low as 0
        SPRegenEfficiency = Mathf.Clamp(SPRegenEfficiency, 100, int.MaxValue);
        
        //Update the Ultimate Cooldown
        if(currentUltimateCooldown > 0)
        {
            currentUltimateCooldown -= Time.deltaTime;
        }

        // Check if the player is attacking
        if(!isAttacking)
        {
            if (playerInput.Game.Fire.triggered)
            {
                BasicAttack();
            }
        }
    }


    #region Captain Skill Set Methods
    public void BasicAttack()
    {   
        //Calculate the Basic Attack Damage
        basicAttackDamage = basicAttackBaseDMG + (basicDamage * basicAttackMultiplier / 100);
        basicAttackDamage = dmgCalulator.BoostDamage(basicAttackDamage);

        // Sent the trigger to the animator coder
        fireTriggered = true;
    }
    public void UltimateAttack()
    {
        //Sent the ultimate damage to the wave of energy prefab
        ultimateDamage = ultimateBaseDamage + (basicDamage * ultimateDamageMultiplier / 100);
        ultimateDamage = dmgCalulator.BoostDamage(ultimateDamage);
        if(isPassiveActive)
        {
            ultimateDamage += ultimateDamage * ultimateUWPassiveBonus / 100;
        }
        //Check if Captain has enough SP and one stack to cast the ultimate
        if(CanCastUltimate() && !isAttacking)
        {
            //Sent the trigger to the animator coder
            ultimateTriggered = true;
            currentSP -= requiredSP;
            //Set the Ultimate Cooldown
            currentUltimateCooldown = ultimateCooldown;

            //Activate the Passive if it is not active, otherwise, remove the passive
            if(isPassiveActive)
            {
                UltRemovePassive();
            }
            else
            {
                StartCoroutine(ActivatePassive());
            }
        }
    }
    //Ultimate Requirement
    public bool CanCastUltimate()
    {
        bool hasEnoughSP = currentSP >= requiredSP;
        bool isCooldownOver = currentUltimateCooldown <= 0;
        bool isOnGround = platformerMovement2D._isGrounded;
        bool canCast = hasEnoughSP && isCooldownOver && isOnGround;
        return canCast;
    }
    //Passive
    private Coroutine passiveCoroutine;
    public bool isAttacking = false;
    private Coroutine sweetSnacksCoroutine;

    private void TriggerPassive() //Implement this method to the Basic Attack damage method
    {
        if(UnityEngine.Random.Range(0, 100) <= basicAttackUWTriggerChance)
        {
            if (passiveCoroutine != null)
            {
                StopCoroutine(passiveCoroutine);
            }
            passiveCoroutine = StartCoroutine(ActivatePassive());
        }
    }
    private void UltRemovePassive()
    {
        StopCoroutine(ActivatePassive());
        dmgCalulator.DecreaseDMGBoost(passiveDMGBoost);
        isPassiveActive = false;
    }
        
    private IEnumerator ActivatePassive()
    {
        if(!isPassiveActive)
        {
            isPassiveActive = true;
            dmgCalulator.IncreaseDMGBoost(passiveDMGBoost);
        }
        yield return new WaitForSeconds(passiveDuration);
        dmgCalulator.DecreaseDMGBoost(passiveDMGBoost);
        isPassiveActive = false;        
    }
    #endregion

    #region Expansion Chips

    #region Sweet Snacks
    private IEnumerator SweetSnacksSPRegen(int time, int amount)
    {
        Debug.Log("Sweet Snacks: Captain is regenerating SP");
        if(currentSP < maxSP)
        {
            currentSP += amount * (SPRegenEfficiency / 100);
        }
        yield return new WaitForSeconds(time);
        sweetSnacksCoroutine = null;
    }
    #endregion
    #endregion

    #region Animation Events
    public void DealDMG()
    {
        //Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        Collider2D[] hitDestroyables = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, destroyableLayers);
        
        if(hitEnemies.Length > 0 || hitDestroyables.Length > 0)
        {
            //SP Regen
            if(currentSP < maxSP)
            {
                currentSP += SPRegenRate * (SPRegenEfficiency / 100);
            }
            TriggerPassive();
            // Debug.Log("Captain's Basic Attack Hit hit enemies or destroyables!");
        }

        // Expansions Chips: Bloodlust
        if(expansionChips._Bloodlust && hitEnemies.Length > 0)
        {
            if(UnityEngine.Random.Range(0, 100) <= expansionChips._BloodlustTriggerChance)
            {
                GetComponent<PlayerHealth>().GainHealth(1);
            }
        }

        //Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            //Critical Rate and Damage
            if (UnityEngine.Random.Range(0, 100) <= criticalRate)
            {
                criticalHit = true;
            }
            else
            {
                criticalHit = false;
            }
            if(!criticalHit)
            {
                enemy.GetComponent<TakeDMG>().TakeMeleeDamage(basicAttackDamage, damageType, DamageFromSkill.BasicAttack);
            }
            else
            {
                float basicAttackCriticalDamage = basicAttackDamage * (criticalDamageMultiplier / 100);
                enemy.GetComponent<TakeDMG>().TakeMeleeDamage(basicAttackCriticalDamage, damageType, DamageFromSkill.BasicAttack);
                criticalHit = false; //This is to reset the critical hit status
            }            
        }   
        foreach (Collider2D destroyable in hitDestroyables)
        {
            destroyable.GetComponent<TakeDMG>().TakeDestroyableDamage(1);
        }
    }
    public void ShootEnergyWave()
    {
        //Calculate the wave direction (left or right) only
        Quaternion waveRotation;
        if(!platformerMovement2D.isLookingRight)
        {
            waveRotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            waveRotation = Quaternion.Euler(0, 0, 0);
        }

        GameObject waveOfEnergy = Instantiate(waveOfEnergyPrefab, transform.position, waveRotation);
        waveOfEnergy.GetComponent<MoonWaveProjectile>().SetWaveDamage(ultimateDamage, damageType);
    }
    #endregion    

    //Gizmos for the attack range
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}