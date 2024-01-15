using Unity.VisualScripting;
using UnityEngine;

// This script is a basic 2D character controller that allows
// the player to run and jump. It uses Unity's new input system,
// which needs to be set up accordingly for directional movement
// and jumping buttons.

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerMovement2D : MonoBehaviour
{

    [Header("Movement Parameters")]
    //Move left or right
    public float runSpeed = 6.0f;
    private bool isFacingRight = true;

    //Jumping
    public Transform groundCheck;
    private readonly float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded = false;
    public float jumpSpeed = 13.0f;
    public float gravityScale = 4.0f;
    public int extraJumpValue = 1;
    private int extraJump;

    //Block Player from moving
    public static bool blocked = false;

    //Call components attached to player
    private Rigidbody2D Character;

    private void Awake()
    {
        Character = GetComponent<Rigidbody2D>();

        Character.gravityScale = gravityScale;
        extraJump = extraJumpValue;
    }

    private void Update()
    {
        //return when the player is not allowed to move
        if (blocked)
        {
            return;
        }

        UpdateIsGrounded();

        HandleHorizontalMovement();

        HandleJumping();
    }

    private void UpdateIsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if(isGrounded)
        {
            extraJump = extraJumpValue;
        }
    }

    private void HandleHorizontalMovement()
    {
        Vector3 scale = transform.localScale;

        
        //Vector2 moveDirection = InputManager.GetInstance().GetMoveDirection();
        //Character.velocity = new Vector2(moveDirection.x * runSpeed, Character.velocity.y);
        //if ((moveDirection.x < 0 && isFacingRight) || (moveDirection.x > 0 && !isFacingRight))
        float moveDirection = Input.GetAxis("Horizontal");
        Character.velocity = new Vector2(moveDirection * runSpeed, Character.velocity.y);
        if ((moveDirection < 0 && isFacingRight) || (moveDirection > 0 && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            scale.x *= -1;
            transform.localScale = scale;
            //Flip Character to face left or right
        }
    }

    private void HandleJumping()
    {
        bool jumpPressed = Input.GetButtonDown("Jump");
        //bool jumpPressed = InputManager.GetInstance().GetJumpPressed();
        if ((isGrounded || extraJump > 0) && jumpPressed)
        {
            Character.velocity = new Vector2(Character.velocity.x, jumpSpeed);
            if(!isGrounded)
            {
                extraJump--;
            }
        }
    }

}