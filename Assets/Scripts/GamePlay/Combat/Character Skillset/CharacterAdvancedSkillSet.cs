using UnityEngine;

public class CharacterAdvancedSkillSet : MonoBehaviour
{
    public static CharacterAdvancedSkillSet instance;

    #region Soaring Wing (Double Jump skill tree)
    [Tooltip("This skill enables double jump")] public bool SoaringWingActive = false;
    private bool isSoaringWingApplied = false;
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
        // Check if Soaring Wing skill is active
        if(SoaringWingActive && !isSoaringWingApplied)
        {
            // Apply Soaring Wing skill
            // Extra jumps
            PlatformerMovement2D.instance.extraJumps += 1;
            isSoaringWingApplied = true;
        }
        else if(!SoaringWingActive && isSoaringWingApplied)
        {
            // Remove Soaring Wing skill
            // Reset extra jumps
            PlatformerMovement2D.instance.extraJumps -= 1;
            isSoaringWingApplied = false;
        }

        // Check if Wall Jump skill is active
        if(WallJumpActive && !isWallJumpApplied)
        {
            // Apply Wall Jump skill
            // Wall jump
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
