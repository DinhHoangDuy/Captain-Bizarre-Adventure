using UnityEngine;

public class ExpansionChipStatus : MonoBehaviour
{
    public static ExpansionChipStatus instance;
    // TODO: Add expansion chip effects here!!!

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
    [HideInInspector] public bool isOverloaded = false;
    private bool overloadDebuffApplied = false;
    #endregion

    #region Energy Generator
    /*
        Chip Effect: If the character has less than 60 SP, gain 1 SP per second
    */
    private PlayerSP playerSP;
    public bool isEnergyGeneratorEquipped = false;
    private float energyGeneratorSPGainDelay = 1;
    private float energyGeneratorThresshold;
    public void SetEnergyGeneratorThresshold(int value)
    {
        energyGeneratorThresshold = value;
        Debug.Log("Energy Generator Thresshold: " + energyGeneratorThresshold);
    }
    #endregion

    #region "Hammer" Chip
    public bool isHammerChipEquipped = false;
    #endregion

    #region Dream Builder Chip
    public bool isDreamBuilderChipEquipped = false;
    public bool isDreamBuilderAvailable;
    public float dreamBuilderPlatformDuration;
    public float dreamBuilderPlatformCooldown;
    public float dreamBuilderPlatformCurrentCooldown;
    #endregion

    #region Wraith Chip
    public bool isWarthChipEquipped;
    #endregion

    #region Macabre Dance Chip
    public bool isMacabreDanceChipEquipped;
    public float MacabreDanceTotalDMGBoost;
    public bool isMacabreDanceActive;
    #endregion
    
    #region Swiftness Expansion Chip
    public bool isSwiftnessChipEquipped;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerSP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSP>(); 
        dreamBuilderPlatformCurrentCooldown = 0;

    }

    private void Update()
    {
        #region Key of Blood Moon Chip Effect
        // Apply the overload debuff if the player has breached the chip amount limit
        if(isOverloaded && !overloadDebuffApplied)
        {
            // The player can't add more chips to the system, and the TotalDMGBoost will be reduced by 10%
            DamageOutCalculator.instance.DecreaseDMGBoost(10);
            overloadDebuffApplied = true;
        }
        else if(!isOverloaded && overloadDebuffApplied)
        {
            DamageOutCalculator.instance.IncreaseDMGBoost(10);
            overloadDebuffApplied = false;
        }
        Debug.Log("isOverClocked: " + isOverclocked);
        Debug.Log("isOverloaded: " + isOverloaded);
        #endregion

        #region Energy Generator Chip Effect
        if(isEnergyGeneratorEquipped)
        {
            // If the Energy Generator Chip is equipped, and the character has less than 60 SP, gain 1 SP per second
            if(playerSP._currentSP < energyGeneratorThresshold && energyGeneratorSPGainDelay <= 0)
            {
                playerSP.IncreaseSPByValue(1);
                energyGeneratorSPGainDelay = 1;
            }
            else
            {
                energyGeneratorSPGainDelay -= Time.deltaTime;
            }
        }
        #endregion

        #region Dream Builder Chip Effect
        if(isDreamBuilderChipEquipped)
        {
            if(dreamBuilderPlatformCurrentCooldown > 0)
            {
                dreamBuilderPlatformCurrentCooldown -= Time.deltaTime;
                isDreamBuilderAvailable = false;
            }
            else
            {
                isDreamBuilderAvailable = true;
            }
        }
        else
        {
            isDreamBuilderAvailable = false;
        }

        #endregion
    }
}