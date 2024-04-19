using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SHG.AnimatorCoder;

// The Animation coder is created by SmallHedgeGame (Youtube)
public class CaptainMoonBladeAnimation : AnimatorCoder
{
    private PlatformerMovement2D platformerMovement2D;
    private CaptainMoonBlade skillset;

    void Start()
    {
        Initialize();
        platformerMovement2D = GetComponent<PlatformerMovement2D>();
        skillset = GetComponent<CaptainMoonBlade>();  
    }

    void Update()
    {
        DefaultAnimation(0);
        if(skillset.fireTriggered)
        {
            Play(new(Animations.StandingATK, true, new()));
            skillset.fireTriggered = false;
        }
        if(skillset.ultimateTriggered)
        {
            platformerMovement2D.rb2d.velocity = Vector2.zero;
            Play(new(Animations.SheatheATK, true, new()));
            skillset.ultimateTriggered = false;
        }

        if (platformerMovement2D.jumpPressed)
        {
            Play(new(Animations.Jump, true));
            platformerMovement2D.jumpPressed = false;
        }
    }
    void FixedUpdate()
    {
        SetBool(Parameters.JUMPING, platformerMovement2D._isGoingUp);
        SetBool(Parameters.GROUNDED, platformerMovement2D.UpdateIsGrounded());
        SetBool(Parameters.MIDAIR, platformerMovement2D._isOnAir);
        SetBool(Parameters.FALLING, platformerMovement2D._isFalling);
        SetBool(Parameters.DASHING, platformerMovement2D._isDashing);
    }
    public override void DefaultAnimation(int layer)
    {
        if(platformerMovement2D._isGrounded)
        {
            if(platformerMovement2D._isRunning)
            {
                Play(new(Animations.Run));
            }
            else
            {
                Play(new(Animations.Idle));
            }
        }

        if(platformerMovement2D._isGoingUp)
        {
            SetLocked(false, layer);
            Play(new(Animations.Jump));
        }
        if(platformerMovement2D._isFalling)
        {
            SetLocked(false, layer);
            Play(new(Animations.Falling));
        }

        // Set some variables to tell the scripts when needed.
        if (GetCurrentAnimation(layer) == Animations.StandingATK || GetCurrentAnimation(layer) == Animations.SheatheATK)
        {
            skillset.isAttacking = true;
            // PlatformerMovement2D.blocked = true;
        }
        else
        {
            skillset.isAttacking = false;
            // PlatformerMovement2D.blocked = false;
        }
    }
}
