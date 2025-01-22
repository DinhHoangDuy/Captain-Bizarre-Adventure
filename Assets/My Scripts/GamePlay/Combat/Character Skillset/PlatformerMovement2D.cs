using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class PlatformerMovement2D : MonoBehaviour, IDataPersistence
{
    public static PlatformerMovement2D instance;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private TrailRenderer trailRenderer;

    private PlayerInput playerInput;
    private BoxCollider2D boxCollider2D;

    private float horizontal;
    private float moveDirection;
    [HideInInspector] public float moveSpeed;
    public float jumpingPower;
    public float characterGravityScale;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    #region Extra Jumps
    public int extraJumps = 1;
    private int extraJumpsCounter;
    public bool doubleJumpUnlocked = false;
    #endregion

    #region Dream Builder Chip
    [Header("Dream Builder Chip")]
    [SerializeField] private GameObject dreamBuilderPlatform;
    [SerializeField] private Transform dreamBuilderPlatformSpawnPoint;
    #endregion

    private bool isFacingRight = true;
    public bool IsLookingRight => isFacingRight;

    private bool wallJumpAnimation = false;
    private bool groundJumpAnimation = false;

    private float HoldPositionDelay;

    public bool inputBlocked = false;
    public bool isTransitingFromTheBottomUp = false;

    #region Wall Slide & Wall Jump
    [Header("Wall Slide & Wall Jump")]
    public bool wallJumpUnlocked = false;
    public bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 1f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(6f, 16f);
    #endregion

    #region Dash
    public bool dashUnlocked = false;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashForce;
    private float dashTime = 0.15f;
    private float dashCooldown = 1f;
    private float dashCurrentCooldown;
    #endregion

    private float _fallSpeedYDampingChangeThreshold;

    [HideInInspector] public Rigidbody2D rb;
    [SerializeField] private Animator anim;
    private MovementStats stats;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] public LayerMask wallLayer;

    // Only for Debugging
    private float currentVelocityX;
    private float currentVelocityY;

    private void OnEnable()
    {
        playerInput.Player.Enable();
    }
    private void OnDisable()
    {
        playerInput.Player.Disable();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<MovementStats>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        coyoteTimeCounter = coyoteTime;
        moveSpeed = stats.MoveSpeed;
        jumpingPower = stats.JumpForce;
        characterGravityScale = stats.GravityScale;
        rb.gravityScale = characterGravityScale;
        dashForce = stats.DashForce;
        dashForce = stats.DashForce;
        extraJumpsCounter = extraJumps;
    }

    private void Update()
    {
        if (isDashing) return;
        horizontal = playerInput.Player.Move.ReadValue<Vector2>().x;
        if (horizontal < 0)
        {
            moveDirection = -1f;
        }
        else if (horizontal > 0)
        {
            moveDirection = 1f;
        }
        else
        {
            moveDirection = 0f;
        }
        #region Vertical Jumping
        if (playerInput.Player.Jump.triggered && !isWallSliding)
        {
            if (coyoteTimeCounter <= 0f)
            {
                if (extraJumpsCounter > 0 && doubleJumpUnlocked)
                {
                    extraJumpsCounter--;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                    // rb.AddForce(new Vector2(rb.linearVelocity.x, jumpingPower), ForceMode2D.Impulse);
                    groundJumpAnimation = true;
                    wallJumpAnimation = false;
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                // rb.AddForce(new Vector2(rb.linearVelocity.x, jumpingPower), ForceMode2D.Impulse);
                groundJumpAnimation = true;
                wallJumpAnimation = false;
            }
        }
        if (playerInput.Player.Jump.WasReleasedThisFrame() && rb.linearVelocity.y > 0 && !isTransitingFromTheBottomUp)
        {
            // Stop Moving Upwards immediately when Jump Button is released
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
            coyoteTimeCounter = -1f;
        }

        if (rb.linearVelocity.y < 0f)
        {
            wallJumpAnimation = false;
            groundJumpAnimation = false;
        }
        #endregion

        if (IsGrounded())
        {
            extraJumpsCounter = extraJumps;
            coyoteTimeCounter = coyoteTime;
            isTransitingFromTheBottomUp = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (rb.linearVelocity.y < -20f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -20f);
        }


        WallSlide();
        WallJump();
        if (!isWallSliding)
        {
            HoldPositionDelay = 0.1f;
        }
        if (!isWallJumping)
        {
            // if (transform.localRotation.y < 0 && moveDirection > 0f || transform.localRotation.y >= 0 && horizontal < 0f)
            if (!isFacingRight && moveDirection > 0f || isFacingRight && horizontal < 0f)
            {
                Flip();
            }
        }

        if (dashUnlocked && playerInput.Player.Dash.triggered && canDash)
        {
            StartCoroutine(Dash());
        }

        if (dashCurrentCooldown > 0f)
        {
            dashCurrentCooldown -= Time.deltaTime;
            canDash = false;
        }
        else
        {
            if (IsGrounded())
            {
                canDash = true;
            }
        }

        // Debug Only
        currentVelocityY = rb.linearVelocityY;
        currentVelocityX = rb.linearVelocityX;
        // Debug.Log("Velocity X: " + currentVelocityX + ". Velocity Y: " + currentVelocityY);
    }

    private void FixedUpdate()
    {
        // if (GetComponent<CaptainSkillSet>().isAttacking || GetComponent<CaptainSkillSet>().isChargingAttack)
        if (GetComponent<CaptainSkillSet>().isAttacking)
        {
            rb.linearVelocityX = 0f; 
            return;
        }
        if (inputBlocked || isDashing || GetComponent<CaptainSkillSet>().isAttacking)
        {
            return;
        }

        if (!isWallJumping)
        {
            // Horizontal Movement
            if (!inputBlocked)
            {
                if (horizontal == 0 && Mathf.Abs(rb.linearVelocityX) <= 2f)
                {
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                }
                else
                {
                    // Set desired speed (velocity) based on input.
                    float targetSpeed = moveDirection * moveSpeed;
                    // Calculate the difference between current velocity and target velocity.
                    float speedChange = targetSpeed - rb.linearVelocityX;
                    // Change acceleration based on the difference.
                    float accelRate = (Mathf.Abs(speedChange) > 0.1f) ? stats.acceleration : stats.deceleration;

                    float movement = Mathf.Pow(Mathf.Abs(speedChange) * accelRate, stats.velocityPower) * Mathf.Sign(speedChange);

                    rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
                }

                // Drafts
                // rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
                // rb.AddForce(new Vector2(moveDirection * moveSpeed, 0), ForceMode2D.Force);
            }
        }


        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = characterGravityScale * 1.5f;
        }
        else
        {
            rb.gravityScale = characterGravityScale;
        }

        #region Animation State
        anim.SetBool("isGrounded", IsGrounded());
        anim.SetBool("isJumping", rb.linearVelocity.y > 0f);
        anim.SetBool("isFalling", rb.linearVelocity.y < 0f);
        anim.SetBool("isRunning", rb.linearVelocity.x != 0f);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isWallJumping", wallJumpAnimation);
        anim.SetBool("isGroundJumping", groundJumpAnimation);
        #endregion
    }

    #region Is Grounded
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);
    }
    #endregion

    #region WallSlide & WallJump
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer) && !IsGrounded();
    }

    private void WallSlide()
    {
        if (!wallJumpUnlocked) return; //Skip Wall Slide if Wall Jump is not Unlocked

        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;

            if (extraJumpsCounter == 0)
            {
                extraJumpsCounter = extraJumps;
            }

            if (HoldPositionDelay >= 0f)
            {
                HoldPositionDelay -= Time.deltaTime;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                // rb.AddForce(new Vector2(rb.linearVelocity.x, 0f), ForceMode2D.Force);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
                // rb.AddForce(new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue)), ForceMode2D.Force);
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (!wallJumpUnlocked) return; //Skip Wall Jump if Wall Jump is not Unlocked
        if (isWallSliding)
        {
            isWallJumping = false;
            // Wall Jumping Direction
            // if (transform.localRotation.y >= 0)
            if (isFacingRight)
            {
                wallJumpingDirection = -1f;
            }
            // else if (transform.localRotation.y < 0)
            else
            {
                wallJumpingDirection = 1f;
            }
            // Wall Jumping Counter
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (playerInput.Player.Jump.triggered && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            wallJumpAnimation = true;
            groundJumpAnimation = false;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            // rb.AddForce(new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y), ForceMode2D.Impulse); 
            wallJumpingCounter = 0f;
            Flip();

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
        trailRenderer.emitting = false;
    }
    #endregion


    public void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    #region Dash
    private IEnumerator Dash()
    {
        anim.SetTrigger("Dash");
        // Debug.Log("Dash!");

        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        float dashDirection;
        if (horizontal < 0)
        {
            dashDirection = -1f;
        }
        else if (horizontal > 0)
        {
            dashDirection = 1f;
        }
        else
        {
            dashDirection = isFacingRight ? 1f : -1f;
        }
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        if (!isFacingRight && dashDirection > 0f || isFacingRight && dashDirection < 0f)
        {
            Flip();
        }

        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        dashCurrentCooldown = dashCooldown;
    }
    #endregion

    #region Animation Events
    public void StartDash()
    {
        rb.gravityScale = 0f;
    }
    public void EndDash()
    {
        rb.gravityScale = characterGravityScale;
    }
    #endregion


    #region Save and Load system
    public void LoadData(GameData data)
    {
        this.doubleJumpUnlocked = data.doubleJumpUnlocked;
        this.wallJumpUnlocked = data.wallJumpUnlocked;
        this.dashUnlocked = data.dashUnlocked;

        this.transform.position = data.lastSavedLocation;
    }

    public void SaveData(ref GameData data)
    {
        data.doubleJumpUnlocked = this.doubleJumpUnlocked;
        data.wallJumpUnlocked = this.wallJumpUnlocked;
        data.dashUnlocked = this.dashUnlocked;
        data.lastSavedLocation = this.transform.position;
    }
    #endregion
    #region  Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
        Gizmos.DrawWireSphere(groundCheck.transform.position, 0.1f);
    }
    #endregion
}