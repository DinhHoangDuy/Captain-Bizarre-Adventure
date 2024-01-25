using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlatformerMovement2D : MonoBehaviour
{
    /*Player components (Some of them will be taken from the character, others will be used for other stuff
        such as directions, etc)*/
    private Rigidbody2D rb2d;
    private BoxCollider2D bc2d;
    private float horizontalInput;
    private bool isLookingRight = true;
    //Player status (movement blocked, movement slowed, health...)
    public static bool blocked = false;
    private bool slowed = false;
    private bool trapped = false;
    private bool movementAllowed = true;

    [Header("Character Parameters")]
    [Tooltip("Speed of the Character")][SerializeField] private float movementSpeed = 6f;
    [Tooltip("How much the character is slowed (from 0.1 to 0.9)")][SerializeField] private float movementSlowEffect = 0.3f;
    private float normalMovementSpeed; //The normal speed, not slowed by any bad status of the character.
    
    [Header("Jumping")]
    [SerializeField] private Transform groundCheck;
    private readonly float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 13.0f;
    private float normaljumpForce;
    [Tooltip("How heavy is the character?")]
    [SerializeField] private float characterGravityScale = 4.0f;

    [Tooltip("Allow the character to jump multiple times")]
    [SerializeField] private int extraJumpValue = 1;
    [Tooltip("How strong is the second jump? (Apply to the second jump only)")]
    [SerializeField] private float doubleJumpForce = 1.5f;
    public static bool isGrounded = false;
    private int extraJump = 0;

    //Animation
    private Animator CharacterAnimator; //Don't need [SerializeField] because this script will take it from the Character
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        CharacterAnimator = GetComponent<Animator>();
    }
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
        if (blocked) return;

        HandleMoveMent();        
        HandleJumping();
        HandleDashing();
        AutoUpdateAnimationState();
    }
    private void FixedUpdate()
    {
        UpdateIsGrounded();
        UpdateIsOnWall();
    }
    private void HandleMoveMent()
    {
        if(!movementAllowed) return;
        //Check player status (slowed, trapped, etc...) 
        BadStatusCheck();

        horizontalInput = Input.GetAxisRaw("Horizontal");
        rb2d.velocity = new Vector2(horizontalInput * movementSpeed, rb2d.velocity.y);
        //Flip Character when moving left or right
        if ((horizontalInput == -1 && isLookingRight) || (horizontalInput == 1 && !isLookingRight))
        {
            /*
            Vector2 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
            */
            transform.Rotate(0f, 180f, 0f);
            isLookingRight = !isLookingRight;
        }
    }
    private void BadStatusCheck()
    {
        if (slowed) //reduce left and right speed
        {
            movementSpeed = movementSpeed * (1 - movementSlowEffect);
        }
        else if (trapped)
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
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(bc2d.bounds.center, bc2d.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool UpdateIsGrounded()
    {
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = IsGrounded();
        if (isGrounded)
        {
            extraJump = extraJumpValue;
            CharacterAnimator.SetTrigger("Landed");
            CharacterAnimator.SetBool("Falling", false);
            CharacterAnimator.SetBool("Jumping Up", false);
        }
        else
        {
            if (rb2d.velocity.y < 0)
            {
                //FallingAnimation();
                CharacterAnimator.SetBool("Falling", true);
                //CharacterAnimator.SetBool("Jumping", false);
                CharacterAnimator.SetBool("Jumping Up", false);
            }
            else if(rb2d.velocity.y > 0)
            {
                CharacterAnimator.SetBool("Jumping Up", true);
            }
        }
        return isGrounded;
    }
    private void UpdateIsOnWall()
    {

    }
    private void HandleJumping()
    {
        bool jumpPressed = Input.GetButtonDown("Jump");        
        if (jumpPressed)
        {
            if (isGrounded)
            {
                Jump(jumpForce);
            }
            else if (extraJump > 0)
            {                
                Jump(jumpForce * doubleJumpForce);
                extraJump--;
            }
        }
    }
    private void Jump( float jumpForce)
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
    }

    //Animate character functions
    private void AutoUpdateAnimationState()
    {
        CharacterAnimator.SetBool("Running", horizontalInput != 0);
        CharacterAnimator.SetBool("Grounded", UpdateIsGrounded());
    }
    private void HandleDashing()
    {
        if(Input.GetButtonDown("Dash"))
        {
            Debug.Log("Player Dashed!");
        }
    }
}