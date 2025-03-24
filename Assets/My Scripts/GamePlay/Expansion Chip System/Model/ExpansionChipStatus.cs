using UnityEngine;

public class ExpansionChipStatus : MonoBehaviour
{
    public static ExpansionChipStatus instance;
    private CaptainSkillSet skillSet;
    private DamageOutCalculator damageOutCalculator;
    private PlayerHealth playerHealth;

    // TODO: Add more expansion chip effects here!!!

    [Header("Positive Buffs")]

    #region Overclock state
    /*
        Overclock effect: Automatically enabled when the Key of Blood Moon Chip is equipped, or when the player has breached the chip load limit
        Ingame effect: The character will receive more damage, and the player won't be able to add more chips to the system (if the Key of Blood Moon Chip is not equipped)
    */

    /*
        Key of Blood Moon Chip Effect: Ignore the load limit, change load limit to chip amount limit,
        and force the system to the overclocked state.

        Breach the chip amount limit: The player can't add more chips to the system, and the TotalDMGBoost will be reduced by 10%      
    */
    [HideInInspector] public bool isKeyOfBloodMoonEquipped = false;
    [HideInInspector] public bool isOverclocked = false;
    #endregion

    #region Sharpened Sword Chip
    /*
        Sharpened Sword Chip Effect: Increase 10 Basic attack value when equipped
    */
    [Header("Sharpened Sword Chip Buff")]
    public bool isSharpenedSwordChipEquipped = false;
    private bool isSharpenedSwordBuffActive = false;
    public float sharpenedSwordChipBuffValue = 10;
    #endregion

    #region Energy Generator
    /*
        Chip Effect: If the character has less than 60 SP, gain 1 SP per second
    */
    [Header("Energy Generator Chip Buff")]
    public bool isEnergyGeneratorEquipped = false;
    private float energyGeneratorSPGainDelay = 1;

    [Tooltip("The SP thresshold to stop gaining more SP")]
    public float energyGeneratorThresshold = 60;
    #endregion

    #region Fortitude Chip
    /*
        Fortitude Chip Effect: If the Fortitude Chip is equipped, the player has +1 Max Health
    */
    [Header("Fortitude Chip Buff")]
    public bool isFortitudeChipEquipped = false;
    public int fortitudeChipBuffValue = 1;
    private bool isFortitudeBuffActive = false;
    #endregion

    #region "Hammer" Chip
    /*
        Hammer Chip Effect: If the Hammer Chip is equipped, Ultimate will cost more SP, but the damage will deal 30% bonus DMG
    */
    [Header("Hammer Chip Buff")]
    public bool isHammerChipEquipped = false;
    #endregion

    #region Wrath Chip
    /*
        Wrath Chip Equipped: If passive "Unbreakable Will" is active, Captain has +20% Crit DMG.
    */
    [Header("Wrath Chip Buff")]
    public bool isWrathChipEquipped;
    public float wrathCritDMGBuffValue = 20f;
    #endregion

    #region Speed Boot Chip
    /*
        Speed Boot Chip Effect: Increase 15% Speed
    */
    [Header("Speed Boot Chip Buff")]
    public bool isSpeedBootChipEquipped = false;
    private bool isSpeedBootBuffActive = false;
    public float speedBootChipBuffValue = 15.0f;
    private float originalSpeed;
    private float speedDifference;
    #endregion

    [Header("Negative Buffs")]
    #region Broken Sword Chip
    [Header("Broken Sword Chip")]
    public bool isBrokenSwordChipEquipped = false;
    private bool isBrokenSwordDebuffActive = false;
    public float brokenSwordChipDebuffValue = 50f;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        skillSet = GameObject.FindGameObjectWithTag("Player").GetComponent<CaptainSkillSet>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        damageOutCalculator = GameObject.FindGameObjectWithTag("Player").GetComponent<DamageOutCalculator>();

        // For speed boots chip
        originalSpeed = PlatformerMovement2D.instance.moveSpeed;
        Debug.Log ("Original Speed is: " + originalSpeed);
        speedDifference = originalSpeed * (speedBootChipBuffValue / 100);
        Debug.Log ("Speed Difference is: " + speedDifference);
    }

    private void Update()
    {
        //===============Positive Buffs================
        #region Sharpened Sword Chip Effect
        if (isSharpenedSwordChipEquipped && !isSharpenedSwordBuffActive)
        {
            skillSet.basicATK += sharpenedSwordChipBuffValue;
            isSharpenedSwordBuffActive = true;
        }
        else if (!isSharpenedSwordChipEquipped && isSharpenedSwordBuffActive)
        {
            skillSet.basicATK -= sharpenedSwordChipBuffValue;
            isSharpenedSwordBuffActive = false;
        }
        #endregion

        #region Fortitude Chip Effect
        if (isFortitudeChipEquipped && !isFortitudeBuffActive)
        {
            playerHealth.maxHealth += fortitudeChipBuffValue;
            isFortitudeBuffActive = true;
        }
        else if (!isFortitudeChipEquipped && isFortitudeBuffActive)
        {
            playerHealth.maxHealth -= fortitudeChipBuffValue;
            isFortitudeBuffActive = false;
        }
        #endregion

        #region Energy Generator Chip Effect
        if (isEnergyGeneratorEquipped)
        {
            // If the Energy Generator Chip is equipped, and the character has less than 60 SP, gain 1 SP per second
            if (PlayerSP.instance._currentSP < energyGeneratorThresshold && energyGeneratorSPGainDelay <= 0)
            {
                PlayerSP.instance.IncreaseSPByValue(1);
                energyGeneratorSPGainDelay = 1;
            }
            else
            {
                energyGeneratorSPGainDelay -= Time.deltaTime;
            }
        }
        #endregion

        #region Speed Boot Chip Effect
        if (isSpeedBootChipEquipped && !isSpeedBootBuffActive)
        {
            PlatformerMovement2D.instance.moveSpeed += speedDifference;
            Debug.Log("Current Moving Speed: " + PlatformerMovement2D.instance.moveSpeed);
            isSpeedBootBuffActive = true;
        }
        else if (!isSpeedBootChipEquipped && isSpeedBootBuffActive)
        {
            PlatformerMovement2D.instance.moveSpeed -= speedDifference;
            Debug.Log("Current Moving Speed: " + PlatformerMovement2D.instance.moveSpeed);
            isSpeedBootBuffActive = false;
        }
        #endregion

        //===============Negative Buffs================
        #region Broken Sword Chip Effect
        // Broken Sword Chip Effect: When equipped, reduce the total DMG Boost by 50%
        if (isBrokenSwordChipEquipped && !isBrokenSwordDebuffActive)
        {
            damageOutCalculator.DecreaseDMGBoost(brokenSwordChipDebuffValue);
            isBrokenSwordDebuffActive = true;
            Debug.Log("Current DMG Boost: " + damageOutCalculator._totalDMGBoost);
        }
        else if (!isBrokenSwordChipEquipped && isBrokenSwordDebuffActive)
        {
            damageOutCalculator.IncreaseDMGBoost(brokenSwordChipDebuffValue);
            isBrokenSwordDebuffActive = false;
            Debug.Log("Current DMG Boost: " + damageOutCalculator._totalDMGBoost);
        }
        #endregion
    }
}