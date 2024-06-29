using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DamageOutCalculator))]
[RequireComponent(typeof(PlatformerMovement2D))]
[RequireComponent(typeof(PlayerHealth))]
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
    - Passive "Unbreakable Will": 
        + Each Basic Attack has a 20%/30%/40% chance to grant "Unbreakable Will" for 5 seconds. This effect will be refreshed if the effect is triggered again during the duration.
        + When "Unbreakable Will" is active, Captain will gain 30% total Damage Boost.
        + "The Vow under the Moon" will deal 10% more damage if "Unbreakable Will" is active.
    */
    #endregion

    #region Captain's Skill Set Attributes
        [Header("Captain's Skill Set Attributes")]
        [Header("Captain's Basic Attributes")]
            public float basicATK = 100;
            [SerializeField] private DamageType damageType = DamageType.Physical;
            [SerializeField] private float criticalRate = 20f;
            [SerializeField] private float criticalDamageMultiplier = 150f;
            [SerializeField] private float attackRate = 2f;
            float nextAttackTime = 0f;
            [SerializeField] private int startSP = 15;
            public int _startSP { get { return startSP; } }
            [SerializeField] private float SPRegenRate = 1;
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
            [SerializeField] private float waveSpeed = 30f;
            [SerializeField] private float waveLifeTime = 0.2f;

            [Tooltip("Ultimate Boost when passive is available")]
            [SerializeField] private float ultimateUWPassiveBonus = 10f;

            [SerializeField] private float ultimateCooldown = 10f;
            [SerializeField] private int requiredSP = 60;
            public int _requiredSP { get { return requiredSP; } }          

        //Passive
        [Header("Captain's Passive Attributes")]
        [SerializeField] private float passiveDuration = 5f;
        [SerializeField] private int passiveDMGBoost = 30;
       
    #endregion

    #region Expansion Chip System
        // Hammer Expansion Chip: Increase the required SP by 25%, and the ultimate damage by 40%
        // Swiftness Expansion Chip: Decrease the required SP by 25%, and decrease the cooldown by 20%
        private bool isRequiredSPIncreased = false;
        private bool isRequiredSPDecreased = false;
        private int originalRequiredSP;
        private float originalUltimateCooldown;
        public void IncreaseRequiredSP(int value)
        {
            requiredSP += (originalRequiredSP * value) / 100;
        }
        public void DecreaseRequiredSP(int value)
        {
            requiredSP -= (originalRequiredSP * value) / 100;
        }
        public void RestoreTheOriginalSPRequirement()
        {
            requiredSP = originalRequiredSP;
        }
        public void DecreaseUltimateCooldown(int value)
        {
            ultimateCooldown -= (originalUltimateCooldown * value) / 100;
        }
        public void RestoreOriginalUltimateCooldown()
        {
            ultimateCooldown = originalUltimateCooldown;
        }

        // Wrath Chip Buff: If passive "Unbreakable Will" is active, Captain deals 20% bonus Crit DMG.
        public bool isWarthChipEquipped = false;
        [HideInInspector] public float WarthCritDMGBuffValue; // Receive the value from the Wrath Chip Buff script
    #endregion 

    #region Script Dependencies
        [Header("Script Dependencies")]
        private PlatformerMovement2D platformerMovement2D;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private LayerMask destroyableLayers;
        private DamageOutCalculator dmgCalulator;
        private PlatformerMovement2D platformerMovement;
        private ExpansionChipStatus expansionChipStatus;
        private Animator anim;
    #endregion

    #region Current Status 
    public static bool blocked = false;
    [SerializeField] private bool isUnbreakableWillActive = false;
    private float basicAttackDamage;
    public bool fireTriggered;
    public float currentSP;

    public bool ultimateTriggered;
    private float ultimateDamage;
    public static float currentUltimateCooldown = 0f;
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
        anim = GetComponent<Animator>();
        platformerMovement = GetComponent<PlatformerMovement2D>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();
    }
    private void Start()    
    {
        // Set the current SP to the start SP
        currentSP = startSP;
        originalRequiredSP = requiredSP;
        originalUltimateCooldown = ultimateCooldown;
        
        //Get a Warning if the wave of energy prefab is not assigned
        if(waveOfEnergyPrefab == null)
        {
            Debug.LogWarning("Wave of Energy Prefab is not assigned to the Captain's Skill Set");
        }
    }
    private void Update()
    {
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

        #region Hammer Expansion Chip
        if(expansionChipStatus.isHammerChipEquipped && !isRequiredSPIncreased)
        {
            IncreaseRequiredSP(25);
            isRequiredSPIncreased = true;
        }
        else if (!expansionChipStatus.isHammerChipEquipped && isRequiredSPIncreased)
        {
            RestoreTheOriginalSPRequirement();
            isRequiredSPIncreased = false;
        }
        #endregion
        #region Swiftness Expansion Chip
        if(expansionChipStatus.isSwiftnessChipEquipped && !isRequiredSPDecreased)
        {
            DecreaseRequiredSP(10);
            DecreaseUltimateCooldown(SwiftnessChip.SwitfChipReduceCooldownValue);
            isRequiredSPDecreased = true;
        }
        else if (!expansionChipStatus.isSwiftnessChipEquipped && isRequiredSPDecreased)
        {
            RestoreTheOriginalSPRequirement();
            isRequiredSPDecreased = false;
        }
        #endregion

        if(expansionChipStatus.isWarthChipEquipped)
        {
            Debug.Log("Warth Chip is equipped. Crit DMG Bonus: " + WarthCritDMGBuffValue + "%");
        }
    }


    #region Captain Skill Set Methods
    public void BasicAttack()
    {   
        //Calculate the Basic Attack Damage
        basicAttackDamage = basicAttackBaseDMG + (basicATK * basicAttackMultiplier / 100);
        basicAttackDamage = dmgCalulator.BoostDamage(basicAttackDamage);

        // Sent the trigger to the animator coder
        if(Time.time >= nextAttackTime)
        {
            fireTriggered = true;  
            nextAttackTime = Time.time + 1f / attackRate;
            anim.SetTrigger("Basic Attack");       
        }
    }
    public void UltimateAttack()
    {
        //Sent the ultimate damage to the wave of energy prefab
        ultimateDamage = ultimateBaseDamage + (basicATK * ultimateDamageMultiplier / 100);
        if(expansionChipStatus.isMacabreDanceActive && expansionChipStatus.isMacabreDanceChipEquipped)
        {
            // Killing enemies resets Ultimate CD. The next Ultimate will have 30% Total DMG Boost
            float OriginalDamage = ultimateDamage;
            ultimateDamage = dmgCalulator.MacabreDanceTotalDMGBoost(ultimateDamage);
            expansionChipStatus.isMacabreDanceActive = false;
        }
        else
        {
            ultimateDamage = dmgCalulator.BoostDamage(ultimateDamage);
        }
        // Check if the Unbreakable Will is active
        if(isUnbreakableWillActive)
        {
            ultimateDamage += ultimateDamage * ultimateUWPassiveBonus / 100;
        }
        // Check if the hit is critical
        bool criticalHit = false;
        if (UnityEngine.Random.Range(0, 100) <= criticalRate)
        {
            criticalHit = true;
        }
        else
        {
            criticalHit = false;
        }
        if(criticalHit)
        {
            ultimateDamage = ultimateDamage * (criticalDamageMultiplier / 100);
            if(isUnbreakableWillActive && criticalHit)
            {
                ultimateDamage += ultimateDamage * WarthCritDMGBuffValue / 100;
            }
            criticalHit = false;
        }

        if(expansionChipStatus.isHammerChipEquipped)
        {
            ultimateDamage += ultimateDamage * HammerChip.hammerChipBuffValue / 100;
        }
        //Check if Captain has enough SP and one stack to cast the ultimate
        if(CanCastUltimate() && !isAttacking)
        {
            anim.SetTrigger("Ultimate");

            //Sent the trigger to the animator coder
            ultimateTriggered = true;
            currentSP -= requiredSP;
            //Set the Ultimate Cooldown
            currentUltimateCooldown = ultimateCooldown;

            //Activate the Passive if it is not active, otherwise, remove the passive
            if(isUnbreakableWillActive)
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
        // bool isOnGround = platformerMovement2D._isGrounded;
        // bool canCast = hasEnoughSP && isCooldownOver && isOnGround;
        bool canCast = hasEnoughSP && isCooldownOver && !blocked && platformerMovement.IsGrounded();
        return canCast;
    }
    //Passive
    private Coroutine passiveCoroutine;
    public bool isAttacking = false;

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
        isUnbreakableWillActive = false;
    }
        
    private IEnumerator ActivatePassive()
    {
        if(!isUnbreakableWillActive)
        {
            isUnbreakableWillActive = true;
            dmgCalulator.IncreaseDMGBoost(passiveDMGBoost);
        }
        yield return new WaitForSeconds(passiveDuration);
        dmgCalulator.DecreaseDMGBoost(passiveDMGBoost);
        isUnbreakableWillActive = false;        
    }
    #endregion

    #region Animation Events
    public void DealDMG()
    {
        //Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        Collider2D[] hitDestroyables = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, destroyableLayers);
        bool criticalHit = false;
        if(hitEnemies.Length > 0 || hitDestroyables.Length > 0)
        {
            //Critical Rate calculation
            if (UnityEngine.Random.Range(0, 100) <= criticalRate)
            {
                criticalHit = true;
            }
            else
            {
                criticalHit = false;
            }

            //SP Regen
            if(currentSP < maxSP)
            {
                currentSP += SPRegenRate * (SPRegenEfficiency / 100);
            }
            TriggerPassive();
            // Debug.Log("Captain's Basic Attack Hit hit enemies or destroyables!");
        }


        //Damage them
        foreach (Collider2D enemy in hitEnemies)
        {          
            if(criticalHit)
            {        
                basicAttackDamage = basicAttackDamage * (criticalDamageMultiplier / 100);
                if(expansionChipStatus.isWarthChipEquipped)
                {
                    basicAttackDamage += basicAttackDamage * WarthCritDMGBuffValue / 100;
                }
            }          
            enemy.GetComponent<TakeDMG>().TakeMeleeDamage(basicAttackDamage, damageType, DamageFromSkill.BasicAttack);
            criticalHit = false;
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
        if(!platformerMovement2D.IsLookingRight)
        {
            waveRotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            waveRotation = Quaternion.Euler(0, 0, 0);
        }

        GameObject waveOfEnergy = Instantiate(waveOfEnergyPrefab, transform.position, waveRotation);
        MoonWaveProjectile waveProjectile = waveOfEnergy.GetComponent<MoonWaveProjectile>();
        waveProjectile.SetWaveDamage(ultimateDamage, damageType);
        waveProjectile.SetSpeed(waveSpeed);
        waveProjectile.SetDuration(waveLifeTime);
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