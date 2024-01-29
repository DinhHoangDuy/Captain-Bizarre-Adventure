using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
//[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlatformerMovement2D : MonoBehaviour
{
    //Adapt the new Input system
    private PlatformerInputAction platformerInputaction;
    private InputAction horizontalInput;
    private float horizontalMovementValue;
    private InputAction jumpInput;
    private InputAction dashInput;

    /*Player components (Some of them will be taken from the character, others will be used for other stuff
        such as directions, etc)*/
    private Rigidbody2D rb2d;
    private CapsuleCollider2D CharacterCollider;
    private Animator CharacterAnimator;

    //Player status (movement blocked, movement slowed, health...)
    public static bool blocked = false;
    private bool trapped = false;
    private bool movementArrested = false;

    [Header("Character Parameters")]
    [Tooltip("Speed of the Character")][SerializeField] private float movementSpeed = 8f;
    [Tooltip("Character Dash Force")][SerializeField] private float dashForce = 2.5f;
    [Header("Jumping")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 16.0f;
    [Tooltip("How heavy is the character?")]
    [SerializeField] private float characterGravityScale = 4f;
    [Tooltip("How strong is the second jump? (Apply to the second jump only)")] [SerializeField] private float doubleJumpForce = 1.5f;

    [Header("Character Controller Dependencies")]
    [SerializeField] private GameObject leftWallCheck;
    [SerializeField] private GameObject rightWallCheck;

    [Header("Character Effects")]
    [SerializeField] private TrailRenderer dashTrailEffect;
    [SerializeField] private TrailRenderer doubleJumpTrailEffect;

    #region Speed
    private bool isRunning = false;
    private bool isLookingRight = true;
    private float normalMovementSpeed;
    private float horizontalMovement;
    #endregion

    #region Jump
    private bool isGrounded = false;
    private int maxAirJumpCount = 1;
    private int jumpCount = 0;
    #endregion

    #region Wall and Ground
    private float normaljumpForce;
    private bool isJumping = false;
    private bool isOnAir = false;
    private bool isFalling = false;
    private bool isOnWall;
    #endregion

    #region Dash
    private bool canDash = true;
    private bool isDashing = false;
    private bool dashedOnAir = false;
    private float dashingTime = 0.17f;
    private float dashingCooldown = 0.9f;
    #endregion


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        CharacterAnimator = GetComponent<Animator>();
        CharacterCollider = GetComponent<CapsuleCollider2D>();
        platformerInputaction = new PlatformerInputAction();
    }
    private void OnEnable()
    {
        horizontalInput = platformerInputaction.Player.Movement;
        horizontalInput.Enable();

        jumpInput = platformerInputaction.Player.Jump;
        jumpInput.performed += Jump;
        jumpInput.Enable();

        dashInput = platformerInputaction.Player.Dash;
        dashInput.performed += Dash;
        dashInput.Enable();
    }
    private void OnDisable()
    {
        horizontalInput.Disable();
        jumpInput.Disable();
        dashInput.Disable();
    }
    // Start is called before the first frame update
    private void Start()
    {
        //Change rb2d Gravity Scale
        rb2d.gravityScale = characterGravityScale;
        //Assign default, normal speed and jump force
        normalMovementSpeed = movementSpeed;
        normaljumpForce = jumpForce;

        //Reset Jump Count
        jumpCount = 0;

        //Disable trail effect
        dashTrailEffect.emitting = false;
        doubleJumpTrailEffect.emitting = false;
    }

    private void Update()
    {
        //No Update if the character is not allowed to move in specific scenario (Dialogue system, intro, etc...)
        if (blocked) return;
        
        if (movementArrested)
        {
            //Not allow Character to move if movementArrested == true
            Move(0);
        }
        else Move(movementSpeed); //Allow character to move normally
    }
    private void FixedUpdate()
    {
        UpdateIsGrounded();
        UpdateIsOnWall();
        AutoUpdateAnimationState();
    }

    //Movement Handling
    //private void Move(float movementSpeed)
    private void Move(float movementSpeed)
    {
        if (movementSpeed == 0)
        {
            isRunning = false;
            return;
        }

        //Read the Movement button values
        horizontalMovementValue = horizontalInput.ReadValue<Vector2>().x;
        if(horizontalMovementValue == 0)
        {
            isRunning = false;
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }
        else
        {
            isRunning = true;
            //Check player status (slowed, trapped, etc...) 
            BadStatusCheck();

            //Move the Character Left or Right
            horizontalMovement = horizontalMovementValue * movementSpeed;
            rb2d.velocity = new Vector2(horizontalMovement, rb2d.velocity.y);   
        }

    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        }
        else if (jumpCount < maxAirJumpCount)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce * doubleJumpForce);
            doubleJumpTrailEffect.emitting = true;
            jumpCount++;
        }        
    }
    private void Dash(InputAction.CallbackContext context)
    {
        if (canDash && !isDashing)
        {
            if(isGrounded || !dashedOnAir)
            {
                StartCoroutine(StartDash());
                if (!isGrounded) dashedOnAir = true;
            }
        }
    }
    private IEnumerator StartDash()
    {
        //Prevent from dashing again while dashing
        canDash = false;
        isDashing = true;
        movementArrested = true;
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0f;       

        //Calculate the dash velocity
        float dashVelocity = movementSpeed * dashForce;

        //Flip the dash velocity if the character is facing left or if the character is not running while still on the ground
        if(!isLookingRight) dashVelocity *= -1;
        if(isGrounded && !isRunning) dashVelocity *= -1;

        //Apply the dash velocity and start the dash trail effect
        rb2d.velocity = new Vector2(dashVelocity, 0f);
        dashTrailEffect.emitting = true;
        
        yield return new WaitForSeconds(dashingTime);
        //Reset the velocity and movement status
        rb2d.gravityScale = originalGravity;
        movementArrested = false;
        isDashing = false;        
        dashTrailEffect.emitting = false;
        //Start the cooldown
        yield return new WaitForSeconds(dashingCooldown);        
        canDash = true;
    }
    public bool UpdateIsGrounded()
    {
        //Check if the character is grounded or not (using BoxCast and CapsuleCollider)
        isGrounded = Physics2D.BoxCast(CharacterCollider.bounds.center, CharacterCollider.bounds.size, 0f, Vector2.down, 0.2f, groundLayer);
        if (isGrounded)
        {
            jumpCount = 0;
            dashedOnAir = false;
        }
        return isGrounded;
    }
    public bool UpdateIsOnWall()
    {
        isOnWall = Physics2D.OverlapCircle(leftWallCheck.transform.position, 0.2f, groundLayer) 
                   || Physics2D.OverlapCircle(rightWallCheck.transform.position, 0.2f, groundLayer);
        if (isOnWall)
        {
            dashedOnAir = false;
        }
        return isOnWall;
    }

    private void AutoUpdateAnimationState()
    {
        //Flip Character while moving left or right
        if ((horizontalMovementValue < 0 && isLookingRight) || (horizontalMovementValue > 0 && !isLookingRight))
        {
            transform.Rotate(0f, 180f, 0f);
            isLookingRight = !isLookingRight;
        }

        //Update the animation state
        isRunning = horizontalMovementValue != 0;
        isJumping = !isGrounded && rb2d.velocity.y > 0;
        isFalling = !isGrounded && rb2d.velocity.y < 0;
        isOnAir = !isGrounded && rb2d.velocity.y == 0;
        if(isOnAir || isFalling) doubleJumpTrailEffect.emitting = false;

        //Skip if the character animator is null
        if (CharacterAnimator == null) return;
        //On the ground animation
        CharacterAnimator.SetBool("Running", isRunning);
        CharacterAnimator.SetBool("Grounded", isGrounded);

        //Jumping animation
        CharacterAnimator.SetBool("Falling", isFalling);
        CharacterAnimator.SetBool("Jumping Up", isJumping);

        //Landing animation
        if (isGrounded)
        {
            CharacterAnimator.SetTrigger("Landed");
        }

    }

    //Status Check
    private void BadStatusCheck()
    {
        //Check if the character is trapped
        if (trapped)
        {
            movementSpeed = 0;
            jumpForce = 0;
        }
        //Reset the character speed if the character is not trapped
        else
        {
            movementSpeed = normalMovementSpeed;
            jumpForce = normaljumpForce;
        }
    }

    //Gizmos only for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftWallCheck.transform.position, 0.2f);
        Gizmos.DrawWireSphere(rightWallCheck.transform.position, 0.2f);
    }
}
