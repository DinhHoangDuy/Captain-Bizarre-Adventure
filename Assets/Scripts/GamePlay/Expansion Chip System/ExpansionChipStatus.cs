using UnityEngine;

public class ExpansionChipStatus : MonoBehaviour
{
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
    [HideInInspector] public bool isOverclocked = false;
    [HideInInspector] public bool isKeyOfBloodMoonEquipped = false;
    [HideInInspector] public bool isChipAmountBreach = false;
    #endregion

    #region Energy Generator
    /*
        Chip Effect: If the character has less than 60 SP, gain 1 SP per second
    */
    private PlayerSP playerSP;
    public bool isEnergyGeneratorEquipped = false;
    private float energyGeneratorSPGainDelay = 1;
    #endregion

    #region "Hammer" Chip
    public bool isHammerChipEquipped = false;
    #endregion

    private void Start()
    {
        playerSP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSP>();
    }

    private void Update()
    {
        #region Key of Blood Moon Chip Effect
        if (isKeyOfBloodMoonEquipped)
        // If the Key of Blood Moon Chip is equipped,
        // ignore the load limit, and force the system to the overclocked state
        {
            isOverclocked = true;
        }
        else
        {
            isOverclocked = false;
        }
        #endregion

        #region Energy Generator Chip Effect
        if(isEnergyGeneratorEquipped)
        {
            // If the Energy Generator Chip is equipped, and the character has less than 60 SP, gain 1 SP per second
            if(playerSP._currentSP < 60 && energyGeneratorSPGainDelay <= 0)
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
    }
}