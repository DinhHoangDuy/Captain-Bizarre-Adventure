using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class CaptainBloodMoonBlade : MonoBehaviour
{
    #region Captain's Skill Set Description
    /*
    Captain's Back Story: 
    - A normal human who fall in love with a vampire. He made a vow to seek his love and to protect her from cruel and evil rules that stop them from being together.
    Captain's Skill Set Description:
    - Basic Damage: 100
    - Critical Rate: 20%
    - Critical Damage: 150%
    - Damage type: Physical
    - Blood Moon Blade: Captain's basic attack is a 2-hit in a single button. Each hit deals 10 + 60%/70%/80% of Captain's basic attack damage as physical damage.
    - About the Ultimate skill "The Vow under the Moon":
        + Shoots a wave of energy in a straight line, dealing 150/170 (+ 105% of physical attack) as physical damage to all enemies hit. This attack can't crit.
        + "The Vow under the Moon" will grant the "Unbreakable Will" for 5 seconds if the buff is not active.
        + "Unbreakable Will" will increase the damage of "The Vow under the Moon" by 20%/30%/60%.
    - About the Ultimate skill requirement:
        + The skill will use a stack system.
            * The maximum stack is 3. 
            * The skill will consume 1 stack and 50 SP to cast.
            * Each stack will be generated every 10 seconds.
            * Delay between each stack is 3 second. 
    - Passive "Unbreakable Will": 
        + Each Basic Attack has a 20%/30%/40% chance to grant "Unbreakable Will" for 5 seconds. This effect will be refreshed if the effect is triggered again during the duration.
        + When "Unbreakable Will" is active, the Captain's basic attack will deal 20%/30%/60% more damage.
        + "The Vow under the Moon" will deal 20%/30%/50% more damage if "Unbreakable Will" is active. This Ultimate will be consumed if the wave hits an enemy. 
    */
    #endregion
    #region Test Cases
    /*
        * Basic ATK DMG Output Calculation test case
            - Current DMG: 100
            - Critical Rate: 20%
            - Critical Damage: 150%
            - Basic Attack DMG (no critical): 10 + 60% of 100 = 70
            - Basic Attack DMG (critical): 70 * 150% = 105
            - Total DMG Output (2 hit, no critical): 70 + 70 = 140
            - Total DMG Output (2 hit, 1 critical): 105 + 70 = 175
            - Total DMG Output (2 hit, 2 critical): 105 + 105 = 210
        * Ultimate ATK DMG Output Calculation test case
            - Current DMG: 100
            - Ultimate Base DMG: 150
            - Ultimate Physical Attack Multiplier: 50%
            - Ultimate DMG: 150 + (100 * 105%) = 255
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
            [SerializeField] private float SPRegenRate = 1.5f;
            [SerializeField] private int maxSP = 70;

        [Header("Captain's Basic Attack Attributes")]
            [SerializeField] private int basicAttackBaseDMG = 10;
            [SerializeField] private float basicAttackMultiplier = 60f;
            [SerializeField] private float passiveTriggerChance = 20f;

        [Header("Captain's Ultimate Attributes")]
            [SerializeField] private GameObject waveOfEnergyPrefab;
            [SerializeField] private float ultimateBaseDamage = 150;
            [SerializeField] private float ultimateDamageMultiplier = 105f;
            [SerializeField] private int requiredSP = 30;
            //Stack system
            [SerializeField] private int maxUltStack = 3;
            [SerializeField] private float ultStackGenerationTime = 10f;
            [SerializeField] private float ultStackUsageDelay = 3f;            
        
        //Passive
        [Header("Captain's Passive Attributes")]
            [SerializeField] private float passiveDuration = 5f;
            private PlatformerMovement2D platformerMovement2D;
            private bool isUltUsable = true;
    #endregion

    //Current Status
    private bool isPassiveActive = false;
    private float basicAttackDamage;
    private bool criticalHit = false;
    private float currentSP;
    private int currentUltStack = 0;
    private float currentStackRegenCooldown = 0;
    private float ultimateDamage;

    #region Script Dependencies
    [Header("Script Dependencies")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayers;
    private Animator animator;
    #endregion

    #region New Input System
    private PlatformerInputAction platformerInputaction;
    private InputAction FireInput;
    private InputAction UltimateInput;
    

    private void OnEnable()
    {
        platformerInputaction = new PlatformerInputAction();
        FireInput = platformerInputaction.Player.Fire;
        UltimateInput = platformerInputaction.Player.Ultimate;
        FireInput.performed += ctx => FirePressed();
        UltimateInput.performed += ctx => UltimatePressed();
        platformerInputaction.Enable();
    }

    private void FirePressed()
    {
        BasicAttack();
        //throw new NotImplementedException();
    }
    private void UltimatePressed()
    {
        UltimateAttack();
        Debug.Log("Ultimate Pressed");
    }

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        platformerMovement2D = GetComponent<PlatformerMovement2D>();
    }
    private void Start()    
    {
        //Get a Warning if the wave of energy prefab is not assigned
        if(waveOfEnergyPrefab == null)
        {
            Debug.LogWarning("Wave of Energy Prefab is not assigned to the Captain's Skill Set");
        }
        currentSP = startSP;
        //Stack system
        currentUltStack = maxUltStack;
    }
    private void Update()
    {
        //Limit the SP to the maxSP
        if (currentSP > maxSP)
        {
            currentSP = maxSP;
        }
        //Generate a stack every 10 seconds
        GenerateUltStack();
    }
        

    #region Captain Skill Set Methods
    private void BasicAttack()
    {   
        //Calculate the Basic Attack Damage
        basicAttackDamage = 10 + (basicDamage * basicAttackMultiplier / 100);
        //Play the animation
        animator.SetTrigger("Basic Attack");
    }
    private void UltimateAttack()
    {
        //Sent the ultimate damage to the wave of energy prefab
        ultimateDamage = ultimateBaseDamage + (basicDamage * ultimateDamageMultiplier / 100);

        //Check if Captain has enough SP and one stack to cast the ultimate
        if(CanCastUltimate())
        {
            //Consume the stack and SP
            currentUltStack--;
            currentSP -= requiredSP;
            //Play the animation
            animator.SetTrigger("Ultimate");
            //Delay the ultimate attack in 3 seconds
            StartCoroutine(DelayUltimateAttack());
        }
        //animator.SetTrigger("Ultimate");
    }
    //Stack system
    //Check if Captain has enough SP and one stack to cast the ultimate
    private bool CanCastUltimate()
    {
        if(currentSP >= requiredSP && currentUltStack > 0 && isUltUsable)
        {
            return true;
        }
        else
        {
            if(currentSP < requiredSP)
            {
                Debug.Log("Ultimate is not usable yet: Not enough SP to cast the ultimate");
            }
            if(currentUltStack <= 0)
            {
                Debug.Log("Ultimate is not usable yet: Not enough stack to cast the ultimate");
            }
            if(!isUltUsable)
            {
                Debug.Log("Ultimate is not usable yet: Ultimate is delayed by the stack usage delay time");
            }
            return false;
        }
    }
    //Generate a stack every 10 seconds. Attach this to the Update method
    private void GenerateUltStack()
    {
        if(currentUltStack < maxUltStack)
        {
            if(currentStackRegenCooldown <= 0)
            {
                currentUltStack++;
                currentStackRegenCooldown = ultStackGenerationTime;
            }
            else
            {
                currentStackRegenCooldown -= Time.deltaTime;
            }
        }
        if(currentUltStack >= maxUltStack)
        {
            currentUltStack = maxUltStack;
            currentStackRegenCooldown = ultStackGenerationTime;
        }
    }
    //Delay the ultimate attack
    private IEnumerator DelayUltimateAttack()
    {
        isUltUsable = false;
        yield return new WaitForSeconds(ultStackUsageDelay);
        isUltUsable = true;
    }

    #endregion

    #region Animation Events
    public void DealDMG()
    {
        //Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        if(hitEnemies.Length > 0)
        {
            //SP Regen
            if(currentSP < maxSP)
            {
                currentSP += SPRegenRate;
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
                enemy.GetComponent<DMGInput>().TakeMeleeDamage(basicAttackDamage, damageType);
            }
            else
            {
                enemy.GetComponent<DMGInput>().TakeMeleeDamage(basicAttackDamage * (criticalDamageMultiplier / 100), damageType);
                criticalHit = false; //This is to reset the critical hit status
            }            
            enemy.GetComponent<DMGInput>().TakeDestroyableDamage(1);
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
        waveOfEnergy.GetComponent<MoonWaveProjectile>().SetWaveDamage(ultimateDamage);
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
