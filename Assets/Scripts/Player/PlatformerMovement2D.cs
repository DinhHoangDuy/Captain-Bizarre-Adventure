using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
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
    private BoxCollider2D bc2d;
    private Animator CharacterAnimator;
    private TrailRenderer tr;

    //Player status (movement blocked, movement slowed, health...)
    public static bool blocked = false;
    private bool slowed = false;
    private bool trapped = false;
    private bool movementArrested = false;

    [Header("Character Parameters")]
    [Tooltip("Speed of the Character")][SerializeField] private float movementSpeed = 8f;
    [Tooltip("Character Dash Force")][SerializeField] private float dashForce = 24f;
    [Header("Jumping")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 16.0f;
    [Tooltip("How heavy is the character?")]
    [SerializeField] private float characterGravityScale = 3f;
    [Tooltip("Character Gravity Scale while be scaled by this number for heavier fall")] [SerializeField] private float characterFallingGravityScale = 1.5f;

    [Tooltip("Allow the character to jump multiple times")]
    [SerializeField] private int extraJumpValue = 1;
    [Tooltip("How strong is the second jump? (Apply to the second jump only)")]
    [SerializeField] private float doubleJumpForce = 1.5f;

    [Header("Character Controller Dependencies")]
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private GameObject wallCheck;

    #region Speed Check
    private bool isRunning = false;
    private bool isLookingRight = true;
    private float normalMovementSpeed;
    private float horizontalMovement;
    #endregion

    #region Jump Check
    public static bool isGrounded = false;
    private int extraJump = 0;
    private float normaljumpForce;
    #endregion

    #region Dash Check
    private bool canDash = true;
    private bool isDashing = false;    
    private float dashingTime = 0.2f;
    private float dashingDelay = 0.2f;
    private int multipleDash = 1;
    private int dashCount;
    private float dashingCooldown = 1f;
    #endregion


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        CharacterAnimator = GetComponent<Animator>();
        tr = GetComponent<TrailRenderer>();
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

        //Add extraJump to allow character to do multiple jumps
        extraJump = extraJumpValue;
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
        //UpdateIsOnWall();
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
        else if (extraJump > 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce * doubleJumpForce);
            tr.emitting = true;
            extraJump--;
        }        
    }
    private void Dash(InputAction.CallbackContext context)
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(StartDash());
        }
    }
    private IEnumerator StartDash()
    {
        canDash = false;
        isDashing = true;
        movementArrested = true;
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0f;

        //Calculate the dash velocity
        float dashVelocity = movementSpeed * dashForce;

        if(!isLookingRight) dashVelocity *= -1;
        if(isGrounded && !isRunning) dashVelocity *= -1;
        /*
        if (!isRunning && isLookingRight && isGrounded || isRunning && !isLookingRight && isGrounded)
        {
            dashVelocity *= -1;
        }
        */
        rb2d.velocity = new Vector2(dashVelocity, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb2d.gravityScale = originalGravity;
        movementArrested = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    private bool UpdateIsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.2f, groundLayer);
        if (isGrounded)
        {
            extraJump = extraJumpValue;
            isDashing = false;
        }
        return isGrounded;
    }

    private void AutoUpdateAnimationState()
    {
        isRunning = horizontalMovementValue != 0;
        CharacterAnimator.SetBool("Running", isRunning);
        CharacterAnimator.SetBool("Grounded", isGrounded);

        //Flip Character while moving left or right
        if ((horizontalMovementValue < 0 && isLookingRight) || (horizontalMovementValue > 0 && !isLookingRight))
        {
            transform.Rotate(0f, 180f, 0f);
            isLookingRight = !isLookingRight;
        }

        if (isGrounded)
        {
            CharacterAnimator.SetTrigger("Landed");
            CharacterAnimator.SetBool("Falling", false);
            CharacterAnimator.SetBool("Jumping Up", false);
        }
        else
        {
            if (rb2d.velocity.y < 0)
            {
                CharacterAnimator.SetBool("Falling", true);
                CharacterAnimator.SetBool("Jumping Up", false);
                tr.emitting = false;
            }
            else if (rb2d.velocity.y > 0)
            {
                CharacterAnimator.SetBool("Jumping Up", true);
            }
        }
    }

    //Status Check
    private void BadStatusCheck()
    {
        if (trapped)
        {
            movementSpeed = 0;
            jumpForce = 0;
        }
        else //reset movement status
        {
            movementSpeed = normalMovementSpeed;
            jumpForce = normaljumpForce;
        }
    }
}
