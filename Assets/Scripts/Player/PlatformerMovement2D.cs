using System.Collections;
using System.Collections.Generic;
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
    private SpriteRenderer spriteRenderer;
    private float movement;
    //Player status (movement blocked, movement slowed, health...)
    public static bool blocked = false;
    private bool slowed = false;
    private bool trapped = false;

    [Header("Character Parameters")]
    [Tooltip("Speed of the Character")] [SerializeField] private float movementSpeed = 6f;
    [Tooltip("How much the character is slowed (from 0.1 to 0.9)")] [SerializeField] private float movementSlowEffect = 0.3f;
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
    private bool isGrounded = false;
    private int extraJump = 0;

    //Animation
    private Animator CharacterAnimator; //Don't need [SerializeField] because this script will take it from the Character
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if(blocked) return;

        HandleMoveMent();
        UpdateIsGrounded();
        HandleJumping();
    }
    private void HandleMoveMent()
    {
        //Check player status (slowed, trapped, etc...) 
        BadStatusCheck();

        movement = Input.GetAxisRaw("Horizontal");
        CharacterAnimator.SetFloat("Speed", Mathf.Abs(movement));
        rb2d.velocity = new Vector2(movement * movementSpeed, rb2d.velocity.y);
        //Flip Character when moving left or right
        if (movement == -1)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement == 1)
        {
            spriteRenderer.flipX = false;
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
    private void UpdateIsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            extraJump = extraJumpValue;
            LandAnimation();
        }
        else
        {
            if(rb2d.velocity.y < 0)
            {
                FallingAnimation();
            }
        }
    }
    private void HandleJumping()
    {
        bool jumpPressed = Input.GetButtonDown("Jump");
        if (jumpPressed)
        {
            if(isGrounded)
            {
                Jump();
            }
            else if(extraJump > 0)
            {
                Jump();
                extraJump--;
            }
                         
        }
    }
    private void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        JumpAnimation();
    }

    //Animate character functions
    private void JumpAnimation()
    {
        CharacterAnimator.SetBool("Jumping", true);
        CharacterAnimator.SetBool("Grounded", false);        
    }
    private void FallingAnimation()
    {
        CharacterAnimator.SetBool("Falling", true);
        CharacterAnimator.SetBool("Jumping", false);
    }
    private void LandAnimation()
    {
        CharacterAnimator.SetBool("Jumping", false);
        CharacterAnimator.SetBool("Falling", false);
        CharacterAnimator.SetBool("Grounded", true);
    }
}
