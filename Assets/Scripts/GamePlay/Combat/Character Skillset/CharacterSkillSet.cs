using UnityEngine;

public class CharacterSkillSet : MonoBehaviour
{
    public static CharacterSkillSet instance;

    #region Soaring Wing (Double Jump skill tree)
    [Tooltip("This skill enables double jump")] public bool DoubleJumpActive = false;
    private bool isDoubleJumpApplied = false;
    #endregion

    #region Wall Jump (Wall Jump skill tree)
    [Tooltip("This skill enables wall jump")] public bool WallJumpActive = false;
    private bool isWallJumpApplied = false;
    #endregion

    #region Dash (Dash skill tree)
    [Tooltip("This skill enables dash")] public bool DashActive = false;
    private bool isDashApplied = false;
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
        if(DoubleJumpActive && !isDoubleJumpApplied)
        {
            // Apply Double Jump skill
            PlatformerMovement2D.instance.doubleJumpAllowed = true;
            isDoubleJumpApplied = true;
        }
        else if(!DoubleJumpActive && isDoubleJumpApplied)
        {
            // Remove Double Jump skill 
            PlatformerMovement2D.instance.doubleJumpAllowed = false;
            isDoubleJumpApplied = false;
        }

        // Check if Wall Jump skill is active
        if(WallJumpActive && !isWallJumpApplied)
        {
            PlatformerMovement2D.instance.wallJumpAllowed = true;
            isWallJumpApplied = true;
        }
        else if(!WallJumpActive && isWallJumpApplied)
        {
            // Remove Wall Jump skill
            // Disable wall jump
            PlatformerMovement2D.instance.wallJumpAllowed = false;
            isWallJumpApplied = false;
        }

        // Check if Dash skill is active
        if(DashActive && !isDashApplied)
        {
            // Apply Dash skill
            // Dash
            PlatformerMovement2D.instance.dashAllowed = true;
            isDashApplied = true;
        }
        else if(!DashActive && isDashApplied)
        {
            // Remove Dash skill
            // Disable dash
            PlatformerMovement2D.instance.dashAllowed = false;
            isDashApplied = false;
        }
    }
}
