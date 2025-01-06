using UnityEngine;

public class CaptainUnlockableSkillStatus: MonoBehaviour, IDataPersistence
{

    // This script is attached to the player character, which will be used to check if the unlockable skills are active or not
    public static CaptainUnlockableSkillStatus instance;

    [Tooltip("This skill enables using Ultimate")] public bool UltimateActive = false;
    [Tooltip("This skill enables dash")] public bool DashActive = false;
    [Tooltip("This skill enables wall jump")] public bool WallJumpActive = false;
    [Tooltip("This skill enables double jump")] public bool DoubleJumpActive = false;

    #region Game Data
    public void LoadData(GameData data)
    {
        UltimateActive = data.ultimateUnlocked;
        DashActive = data.dashUnlocked;
        WallJumpActive = data.wallJumpUnlocked;
        DoubleJumpActive = data.doubleJumpUnlocked;
    }

    public void SaveData(ref GameData data)
    {
        data.ultimateUnlocked = UltimateActive;
        data.dashUnlocked = DashActive;
        data.wallJumpUnlocked = WallJumpActive;
        data.doubleJumpUnlocked = DoubleJumpActive;
    }
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        // No need to check Ultimate skill, because it is checked frequently in the CaptainSkillSet script

        // Check if Double Jump skill is active
        if(DoubleJumpActive)
        {
            // Apply Double Jump skill
            PlatformerMovement2D.instance.doubleJumpUnlocked = true;
        }
        else
        {
            // Remove Double Jump skill 
            PlatformerMovement2D.instance.doubleJumpUnlocked = false;
        }

        // Check if Wall Jump skill is active
        if(WallJumpActive)
        {
            PlatformerMovement2D.instance.wallJumpUnlocked = true;
        }
        else
        {
            // Remove Wall Jump skill
            // Disable wall jump
            PlatformerMovement2D.instance.wallJumpUnlocked = false;
        }

        // Check if Dash skill is active
        if(DashActive)
        {
            // Apply Dash skill
            // Dash
            PlatformerMovement2D.instance.dashUnlocked = true;
        }
        else 
        {
            // Remove Dash skill
            // Disable dash
            PlatformerMovement2D.instance.dashUnlocked = false;
        }
    }
}
