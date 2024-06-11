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
    //Components
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject rightWallCheck;
    private PlayerInput playerInput;
    private InputAction wasd;
    public static Rigidbody2D rb;
    public static Animator anim;

    #region Current State
    private CharacterStats characterStats;
    private float moveSpeed;
    private float jumpForce;
    private float gravityScale;
    private int extraJumps;
    private int currentExtraJumps;

    // Wall Jump
    [Header("Wall Jump Settings")]
    private bool isWallSliding = false;
    private float wallSlidingSpeed = 2f;
    private float wallJumpDuration = 0.2f;
    [SerializeField] private Vector2 wallJumpingPower;
    private bool isWallJumping = false;
    private float wallJumpingDirection;

    // Dash
    [Header ("Dash Settings")]
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower;
    private float dashingDuration = 0.2f;
    private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer dashTrail;
    
    #endregion

    // State
    private bool isLookingRight = true;
    public bool IsLookingRight { get { return isLookingRight; } }
    public static bool blocked = false;
    private float coyoteTime = 0.2f;


    void OnEnable()
    {
        playerInput.Enable();
    }
    void OnDisable()
    {
        playerInput.Disable();
    }

    void Awake()
    {
        playerInput = new PlayerInput();
        wasd = playerInput.Game.WASD;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    
    void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        moveSpeed = characterStats.MoveSpeed;
        jumpForce = characterStats.JumpForce;
        gravityScale = characterStats.GravityScale;
        extraJumps = characterStats.ExtraJumps;

        currentExtraJumps = extraJumps;

        dashingPower = characterStats.DashForce;
    }

    void Update()
    {
        if(blocked)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if(isDashing)
        {
            return;
        }

        #region Horizontal Movement
        if(playerInput.Game.WASD.ReadValue<Vector2>().x != 0)
        {
            float horizontalInput = wasd.ReadValue<Vector2>().x;
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if(rb.velocity.x > 0 && !isLookingRight)
        {
            Flip();
        }
        else if(rb.velocity.x < 0 && isLookingRight)
        {
            Flip();
        }
        #endregion

        #region Jumping
        rb.gravityScale = gravityScale;
        if(playerInput.Game.Jump.triggered)
        {
            if(coyoteTime > 0 || currentExtraJumps > 0 && !isWallSliding)
            {
                if(!IsGrounded())
                {
                    currentExtraJumps--;
                }
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if(isWallSliding)
            {
                isWallJumping = true;
                if(isLookingRight)
                {
                    rb.velocity = new Vector2(-wallJumpingPower.x, wallJumpingPower.y);
                }
                else
                {
                    rb.velocity = new Vector2(wallJumpingPower.x, wallJumpingPower.y);
                }
                Invoke(nameof(StopWallJumping), wallJumpDuration);
            }
        }
        #endregion
        #region Falling
        if(rb.velocity.y < -30)
        {
            rb.velocity = new Vector2(rb.velocity.x, -30);
        }
        else if(rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * 1.5f;
        }
        else rb.gravityScale = gravityScale;
        #endregion

        #region Dash
        if(playerInput.Game.Dash.triggered && canDash)
        {
            StartCoroutine(Dash());
        }
        #endregion

        #region Grounded Logic
        // Coyote Time
        if(!IsGrounded())
        {
            coyoteTime -= Time.deltaTime;
        }
        else
        {
            coyoteTime = 0.2f;
            currentExtraJumps = extraJumps;
        }
        #endregion

        IsOnWall();
        WallSlide();
    }
    void FixedUpdate()
    {
        #region Animation State
        anim.SetBool("isGrounded", IsGrounded());
        anim.SetBool("isJumping", rb.velocity.y > 0f);
        anim.SetBool("isFalling", rb.velocity.y < 0f);
        anim.SetBool("isRunning", rb.velocity.x != 0);
        Debug.Log("rb.velocity.y < 0 =>" + (rb.velocity.y < 0));
        #endregion
    }

    void Flip()
    {
        isLookingRight = !isLookingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        return hit.collider != null;
    }

    #region Wall Slide and Jump
    bool IsOnWall()
    {
        bool isOnWall = Physics2D.OverlapCircle(rightWallCheck.transform.position, 0.2f, groundLayer);
        return isOnWall;
    }
    void WallSlide()
    {
        if(IsOnWall() && !IsGrounded() && playerInput.Game.WASD.ReadValue<Vector2>().x != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));         
        }
        else
        {
            isWallSliding = false;
        }
    }
    void StopWallJumping()
    {
        isWallJumping = false;
    }
    #endregion
    
    #region Dash
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        dashTrail.emitting = true;

        float dashDirection = 1;
        if (isLookingRight)
        {
            dashDirection = 1;
        }
        else
        {
            dashDirection = -1;
        }
        rb.velocity = new Vector2(dashDirection * dashingPower, 0f);
        yield return new WaitForSeconds(dashingDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
        dashTrail.emitting = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;

    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        // Ground Check Gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1f);

        // Wall Check Gizmo
        Gizmos.DrawWireSphere(rightWallCheck.transform.position, 0.1f);
    }
    #endregion
}