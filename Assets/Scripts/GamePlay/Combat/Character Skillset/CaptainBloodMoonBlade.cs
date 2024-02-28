using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoostDMG))]
[RequireComponent(typeof(PlatformerMovement2D))]
[RequireComponent(typeof(PlayerHealth))]
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
    - About the Ultimate skill requirement:
        + The skill will use a stack system.
            * The skill will consume 1 stack and 50 SP to cast.
    - Passive "Unbreakable Will": 
        + Each Basic Attack has a 20%/30%/40% chance to grant "Unbreakable Will" for 5 seconds. This effect will be refreshed if the effect is triggered again during the duration.
        + When "Unbreakable Will" is active, Captain will gain 30% total Damage Boost.
        + "The Vow under the Moon" will deal 10% more damage if "Unbreakable Will" is active. This Ultimate will be consumed if the wave hits an enemy. 
    - Skill Tree:
        + "Blood for Blood!": When enabled, the Ultimate "The Vow under the Moon" will consume one heart stack to deal 10% more damage. This effect won't trigger if Captain has 1 heart stack left.
        + "Boiling Blood": When enabled, the more heart stacks Captain lost, the his basic damage will increase by 5% per stack lost (max 5 stacks). 
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
            [SerializeField] private float ultimatePassiveBonus = 10f;
            [SerializeField] private int requiredSP = 30;           
        
        //Passive
        [Header("Captain's Passive Attributes")]
            [SerializeField] private float passiveDuration = 5f;
            [SerializeField] private int passiveDMGBoost = 30;
            private PlatformerMovement2D platformerMovement2D;
    #endregion

    #region Script Dependencies
        [Header("Script Dependencies")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private SPBar SPBar;
        private Animator animator;
        private BoostDMG boostDMG;
    #endregion
    //Current Status
    private bool isPassiveActive = false;
    float basicAttackDamage;
    private bool criticalHit = false;
    public float currentSP { get; private set;}
    private float ultimateDamage;    

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
        boostDMG = GetComponent<BoostDMG>();
    }
    private void Start()    
    {
        //Get a Warning if the wave of energy prefab is not assigned
        if(waveOfEnergyPrefab == null)
        {
            Debug.LogWarning("Wave of Energy Prefab is not assigned to the Captain's Skill Set");
        }

        //Get a Warning if the SP Bar is not assigned
        if(SPBar == null)
        {
            Debug.LogWarning("SP Bar is not assigned to the Captain's Skill Set");
        }
        //Set the SP Bar
        SPBar.SetMaxSP(maxSP);
        //Set the start SP
        currentSP = startSP;
        SPBar.SetStartSP(startSP);
    }
    private void Update()
    {
        //Update the SP Bar
        SPBar.SetSP(currentSP);
        SPBar.SetSPText(currentSP);
        //Limit the SP to the maxSP
        if (currentSP > maxSP)
        {
            currentSP = maxSP;
        }
    }

    #region Captain Skill Set Methods
    private void BasicAttack()
    {   
        //Calculate the Basic Attack Damage
        basicAttackDamage = basicAttackBaseDMG + (basicDamage * basicAttackMultiplier / 100);
        basicAttackDamage = boostDMG.BoostDamage(basicAttackDamage);
        /*
            Test Case
            - Captain's basic damage is 100
            - The basic attack base damage is 10
            - The basic attack multiplier is 60%
            - The Passive will add 30% total damage boost
            - Basic attack damage (no passive, no critical) = 10 + (100 * 60%) = 70
            - Basic attack damage (with passive, no critical) = 70 + (70 * 30%) = 91
            - Critical Rate is 20%
            - Critical Damage Multiplier is 150%
            - Basic attack damage (no passive, with critical) = 70 * 150% = 105
            - Basic attack damage (with passive, with critical) = 91 * 150% = 136.5
        */
        //Play the animation
        animator.SetTrigger("Basic Attack");
    }
    private void UltimateAttack()
    {
        //Sent the ultimate damage to the wave of energy prefab
        ultimateDamage = ultimateBaseDamage + (basicDamage * ultimateDamageMultiplier / 100);
        ultimateDamage = boostDMG.BoostDamage(ultimateDamage);
        if(isPassiveActive)
        {
            ultimateDamage += ultimateDamage * ultimatePassiveBonus / 100;
        }
        /*
            Test Case:
            - Captain's basic damage is 100
            - The ultimate base damage is 150
            - The ultimate damage multiplier is 105%
            - Ultimate Damage without passive = 150 + (100 * 105%) = 255
            - The passive will add 10% more damage to the Ultimate Damage if it is active
            - Ultimate Damage with passive = 255 + (255 * 10%) = 280.5
        */


        //Check if Captain has enough SP and one stack to cast the ultimate
        if(CanCastUltimate())
        {
            currentSP -= requiredSP;
            //Play the animation
            animator.SetTrigger("Ultimate");

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
    private bool CanCastUltimate()
    {
        if(currentSP >= requiredSP)
        {
            return true;
        }
        else
        {
            Debug.Log("Not enough SP to cast the Ultimate");
            return false;
        }
    }
    //Passive
    private Coroutine passiveCoroutine;

    private void TriggerPassive() //Implement this method to the Basic Attack damage method
    {
        if(UnityEngine.Random.Range(0, 100) <= passiveTriggerChance)
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
        isPassiveActive = false;
        Debug.Log("Unbreakable Will is inactive: Passive is removed by the Ultimate skill");
    }
        
    private IEnumerator ActivatePassive()
    {
        if(!isPassiveActive)
        {
            isPassiveActive = true;
            boostDMG.IncreaseDMGBoost(passiveDMGBoost);
            Debug.Log("Unbreakable Will is active");
        }
        else if(isPassiveActive)
        {
            Debug.Log("Unbreakable Will is refreshed");
        }
        yield return new WaitForSeconds(passiveDuration);
        boostDMG.DecreaseDMGBoost(passiveDMGBoost);
        isPassiveActive = false;        
        Debug.Log("Unbreakable Will is inactive: Passive is removed by the duration");
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
            TriggerPassive();
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
                enemy.GetComponent<TakeDMG>().TakeMeleeDamage(basicAttackDamage, damageType);
            }
            else
            {
                float basicAttackCriticalDamage = basicAttackDamage * (criticalDamageMultiplier / 100);
                enemy.GetComponent<TakeDMG>().TakeMeleeDamage(basicAttackCriticalDamage, damageType);
                criticalHit = false; //This is to reset the critical hit status
            }            
            enemy.GetComponent<TakeDMG>().TakeDestroyableDamage(1);
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
