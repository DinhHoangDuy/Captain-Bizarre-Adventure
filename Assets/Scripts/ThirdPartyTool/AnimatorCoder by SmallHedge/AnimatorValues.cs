//Author: Small Hedge Games
//Date: 05/04/2024

namespace SHG.AnimatorCoder
{
    /// <summary> Complete list of all animation state names </summary>
    public enum Animations
    {
        Idle,
        Run,
        StandingATK,
        SheatheATK,
        Jump,
        Falling,
        MidAir,
        Hit,
        Death,
        RESET
    }

    /// <summary> Complete list of all animator parameters </summary>
    public enum Parameters
    {
        GROUNDED,
        JUMPING,
        FALLING,
        MIDAIR,
        DASHING
    }
}


