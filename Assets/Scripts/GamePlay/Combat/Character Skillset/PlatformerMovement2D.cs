using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterStats))]
public class PlatformerMovement2D : MonoBehaviour
{
    //Adapt the new Input system
    private PlayerInput playerInput;
    private InputAction horizontalInput;
    private float horizontalMovementValue;
    private InputAction jumpInput;
    private InputAction dashInput;

    /*Player components (Some of them will be taken from the character, others will be used for other stuff
        such as directions, etc)*/
    [HideInInspector]public static Rigidbody2D rb2d;
    public Rigidbody2D _rigidbody2D { get { return rb2d; } }
    private BoxCollider2D CharacterCollider;
    private CharacterStats characterStats;
    private CaptainMoonBlade skillset;
    //Player status (movement blocked, movement slowed, health...)
    public static bool blocked = false;
    private bool trapped = false;
    private const float coyoteTime = 0.2f;
    private float coyoteTimeCounter;


    // Character Movement Variables
    private float movementSpeed;
    private float dashForce;
    [Header("Jumping")]
    [SerializeField] private LayerMask groundLayer;
    private float jumpForce;
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
    public bool _isRunning { get { return isRunning; } }
    [HideInInspector] public bool isLookingRight = true;
    private float normalMovementSpeed;
    private float horizontalMovement;
    #endregion

    #region Jump
    private bool isGrounded = false;
    public bool _isGrounded { get { return isGrounded; } }
    public bool jumpPressed;
    private int maxAirJumpCount = 1;
    private int airJumpCount = 0;
    #endregion

    #region Falling
    private bool isFalling = false;
    public bool _isFalling { get { return isFalling; } }
    private bool isGoingUp = false;
    public bool _isGoingUp { get { return isGoingUp; } }

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
    public bool _isDashing { get { return isDashing; } }
    private bool dashedOnAir = false;
    private float dashingTime = 0.17f;
    private float dashingCooldown = 0.5f;
    public bool dashPressed = false;
    private bool dashBackward = false;

    #endregion


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        CharacterCollider = GetComponent<BoxCollider2D>();
        playerInput = new PlayerInput();
    }

    // This method is called when the object becomes enabled and active.
    // It sets up the input actions for movement, jump, and dash
    private void OnEnable()
    {
        horizontalInput = playerInput.Game.WASD;
        horizontalInput.Enable();

        jumpInput = playerInput.Game.Jump;
        jumpInput.performed += JumpPresed;
        jumpInput.canceled += ResetCoyoteTime;
        jumpInput.Enable();

        dashInput = playerInput.Game.Dash;
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
        characterStats = GetComponent<CharacterStats>();
        skillset = GetComponent<CaptainMoonBlade>();

        //Change rb2d Gravity Scale
        rb2d.gravityScale = characterGravityScale;
        //Assign default, normal speed and jump force
        movementSpeed = characterStats.Speed;
        normalMovementSpeed = movementSpeed;
        jumpForce = characterStats.JumpForce;
        normaljumpForce = jumpForce;
        dashForce = characterStats.DashForce;

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
        if (trapped || skillset.isAttacking)
        {
            //Not allow Character to move if movementArrested == true
            if(skillset.isAttacking) rb2d.velocity = Vector2.zero;
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

        if(isFalling)
        {
            rb2d.gravityScale = fallingGravityScale;
        }
        if(isGrounded || isGoingUp)
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
        // Check if the character is grounded or coyote time is still active. If yes, do the first jump
        if (coyoteTimeCounter > 0f)
        { 
            jumpPressed = true;
            doubleJumpTrailEffect.emitting = false;
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            coyoteTimeCounter = 0;       
        }

        // Check if the character's coyote time is out and the air jump count is less than the max air jump count. If yes, do the second jump
        if (coyoteTimeCounter <= 0 && airJumpCount < maxAirJumpCount)
        {
            jumpPressed = true;
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
        if(skillset.isAttacking == true) return;
        if (canDash && !isDashing)
        {
            if (isGrounded || !dashedOnAir)
            {
                dashPressed = true;
                rb2d.velocity = Vector2.zero;
                if (!isGrounded) dashedOnAir = true; //dashedOnAir is used to prevent the character from dashing again while on air
                StartCoroutine(Dash());
                dashPressed = false;
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
            (this meant for auto dash backward)*/
        if (isGrounded && !isRunning)
        {
            transform.Rotate(0f, 180f, 0f);
            dashVelocity *= -1;
            dashBackward = true;
        }

        //Apply the dash velocity and start the dash trail effect        
        rb2d.velocity = new Vector2(dashVelocity, 0f);
        dashTrailEffect.emitting = true;

        yield return new WaitForSeconds(dashingTime);
        //Reset the velocity and movement status
        if(dashBackward)
        {
            transform.Rotate(0f, 180f, 0f);
            dashBackward = false;
        }
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
        isGoingUp = !isGrounded && rb2d.velocity.y > 0.1;
        isFalling = !isGrounded && rb2d.velocity.y < -0.1;
        if (isFalling) doubleJumpTrailEffect.emitting = false;

    }

    //Gizmos only for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftWallCheck.transform.position, 0.2f);
        Gizmos.DrawWireSphere(rightWallCheck.transform.position, 0.2f);
    }
}