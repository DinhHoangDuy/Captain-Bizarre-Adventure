using UnityEngine;

public class CharacterSkillSet : MonoBehaviour
{
    public static CharacterSkillSet instance;

    #region Soaring Wing (Double Jump skill tree)
    [Tooltip("This skill enables double jump")] public bool DoubleJumpActive = false;
    #endregion

    #region Wall Jump (Wall Jump skill tree)
    [Tooltip("This skill enables wall jump")] public bool WallJumpActive = false;
    #endregion

    #region Dash (Dash skill tree)
    [Tooltip("This skill enables dash")] public bool DashActive = false;
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
