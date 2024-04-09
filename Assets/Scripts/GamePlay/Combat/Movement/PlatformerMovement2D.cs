using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
// [RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlatformerMovement2D : MonoBehaviour
{
    //Adapt the new Input system
    private PlatformerInputAction inputActions;
    private InputAction horizontalInput;
    private float horizontalMovementValue;
    private InputAction jumpInput;
    private InputAction dashInput;

    /*Player components (Some of them will be taken from the character, others will be used for other stuff
        such as directions, etc)*/
    [HideInInspector]public Rigidbody2D rb2d;
    private Animator CharacterAnimator;
    private BoxCollider2D CharacterCollider;

    //Player status (movement blocked, movement slowed, health...)
    public static bool blocked = false;
    private bool trapped = false;
    private const float coyoteTime = 0.2f;
    private float coyoteTimeCounter;


    [Header("Character Parameters")]
    [Tooltip("Speed of the Character")] public float movementSpeed = 8f;
    [Tooltip("Character Dash Force")][SerializeField] private float dashForce = 2.5f;
    [Header("Jumping")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 16.0f;
    [Tooltip("How heavy is the character?")]
    [SerializeField] private float characterGravityScale = 3f;
    [SerializeField] private float fallingGravityScale = 4f;
    [Tooltip("How strong is the second jump? (Apply to the second jump only)")][SerializeField] private float doubleJumpForce = 1.5f;

    [Header("Character Controller Dependencies")]
    [SerializeField] private GameObject leftWallCheck;
    [SerializeField] private GameObject rightWallCheck;
    // [SerializeField] private GameObject groundCheck;

    [Header("Character Effects")]
    [SerializeField] private TrailRenderer dashTrailEffect;
    [SerializeField] private TrailRenderer doubleJumpTrailEffect;

    #region Speed
    private bool isRunning = false;
    [HideInInspector] public bool isLookingRight = true;
    private float normalMovementSpeed;
    private float horizontalMovement;
    #endregion

    #region Jump
    private bool isGrounded = false;
    private int maxAirJumpCount = 1;
    private int airJumpCount = 0;
    #endregion

    #region Falling
    private bool isFalling = false;
    private bool isJumping = false;
    private bool isOnAir = false;

    #endregion

    #region Wall and Ground
    private float normaljumpForce;
    private bool isOnWall;
    //Debugging
    private float currentGravityScale;
    private float currentVelocityY;
    #endregion

    #region Dash
    private bool canDash = true;
    public bool _canDash { get { return canDash; } }
    private bool isDashing = false;
    private bool dashedOnAir = false;
    private float dashingTime = 0.17f;
    private float dashingCooldown = 0.5f;
    #endregion


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        CharacterAnimator = GetComponent<Animator>();
        CharacterCollider = GetComponent<BoxCollider2D>();
        inputActions = new PlatformerInputAction();
    }

    // This method is called when the object becomes enabled and active.
    // It sets up the input actions for movement, jump, and dash
    private void OnEnable()
    {
        horizontalInput = inputActions.Player.Movement;
        horizontalInput.Enable();

        jumpInput = inputActions.Player.Jump;
        jumpInput.performed += JumpPresed;
        jumpInput.canceled += ResetCoyoteTime;
        jumpInput.Enable();

        dashInput = inputActions.Player.Dash;
        dashInput.performed += DashPressed;
        dashInput.Enable();
    }    

    //This method is called when the behaviour becomes disabled or inactive.
    //It disables the input actions for movement, jump, and dash.
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
        airJumpCount = 0;

        //Disable trail effect
        dashTrailEffect.emitting = false;
        doubleJumpTrailEffect.emitting = false;
    }

    private void Update()
    {
        //No Update if the character is not allowed to move in specific scenario (Dialogue system, intro, etc...)
        if (blocked) return;

        #region Trapped
        if (trapped)
        {
            //Not allow Character to move if movementArrested == true
            Move(0);
        }
        else Move(movementSpeed); //Allow character to move normally
        #endregion

        #region Coyote Time        
        if(UpdateIsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        #endregion
        #region Falling Speed
        //Change the gravity scale based on the character status
        if (isOnAir)
        {
            rb2d.gravityScale = characterGravityScale * 0.5f;
        }
        if(isFalling)
        {
            rb2d.gravityScale = fallingGravityScale;
        }
        if(isGrounded || isJumping)
        {
            rb2d.gravityScale = characterGravityScale;
        }
        //Limit the falling speed
        if (rb2d.velocity.y < -40f)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, -40f);
        }
        #endregion
    }
    private void FixedUpdate()
    {
        UpdateIsGrounded();
        UpdateIsOnWall();
        AutoUpdateAnimationState();

        //For Debugging purposes
        currentGravityScale = rb2d.gravityScale;
        currentVelocityY = rb2d.velocity.y;
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
        if (horizontalMovementValue == 0)
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

    private void JumpPresed(InputAction.CallbackContext context)
    {
        if(blocked) return;
        if (coyoteTimeCounter > 0f)
        { 
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);            
        }

        if (coyoteTimeCounter <= 0 && airJumpCount < maxAirJumpCount)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce * doubleJumpForce);
            doubleJumpTrailEffect.emitting = true;
            airJumpCount++;
        }
    }
    private void ResetCoyoteTime(InputAction.CallbackContext context)
    {
        coyoteTimeCounter = 0;
    }
    private void DashPressed(InputAction.CallbackContext context)
    {
        if(blocked) return;
        if (canDash && !isDashing)
        {
            if (isGrounded || !dashedOnAir)
            {
                rb2d.velocity = Vector2.zero;
                if (!isGrounded) dashedOnAir = true; //dashedOnAir is used to prevent the character from dashing again while on air
                StartCoroutine(Dash());
            }
        }
    }
    private IEnumerator Dash()
    {
        //Calculate the dash velocity
        float dashVelocity = movementSpeed * dashForce;

        //Prevent from dashing again while dashing
        canDash = false;
        isDashing = true;
        blocked = true;
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0f;

        //Flip the dash velocity if the character is facing left
        if (!isLookingRight) dashVelocity *= -1;
        /*Flip the dash velocity if the character is on the ground and not running
            (this meant for auto dodging attacks)*/
        if (isGrounded && !isRunning) dashVelocity *= -1;

        //Apply the dash velocity and start the dash trail effect        
        rb2d.velocity = new Vector2(dashVelocity, 0f);
        dashTrailEffect.emitting = true;

        yield return new WaitForSeconds(dashingTime);
        //Reset the velocity and movement status
        rb2d.gravityScale = originalGravity;
        blocked = false;
        isDashing = false;
        dashTrailEffect.emitting = false;
        //Start the cooldown
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    public bool UpdateIsGrounded()
    {
        isGrounded = Physics2D.BoxCast(CharacterCollider.bounds.center, CharacterCollider.bounds.size, 0f, Vector2.down, 0.2f, groundLayer);
        // isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.2f, groundLayer);
        if (isGrounded)
        {
            airJumpCount = 0;
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

    //Animation Handling
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
        isJumping = !isGrounded && rb2d.velocity.y > 0.1;
        isFalling = !isGrounded && rb2d.velocity.y < -0.1;
        //isOnAir returns true if the character is not grounded and velocity.y is in between -0.1 and 0.1
        isOnAir = !isGrounded && (rb2d.velocity.y >= -0.1 && rb2d.velocity.y <= 0.1);
        if (isOnAir || isFalling) doubleJumpTrailEffect.emitting = false;

        //Skip if the character animator is null
        if (CharacterAnimator == null) return;
        //On the ground animation
        CharacterAnimator.SetBool("Running", isRunning);
        CharacterAnimator.SetBool("Grounded", isGrounded);

        //Jumping animation
        CharacterAnimator.SetBool("Falling", isFalling);
        CharacterAnimator.SetBool("Jumping Up", isJumping);
        CharacterAnimator.SetBool("Mid Air", isOnAir);
    }

    //Gizmos only for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftWallCheck.transform.position, 0.2f);
        Gizmos.DrawWireSphere(rightWallCheck.transform.position, 0.2f);
        // Gizmos.DrawWireSphere(groundCheck.transform.position, 0.2f);
    }
}
