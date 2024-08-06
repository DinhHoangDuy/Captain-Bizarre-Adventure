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
    private ExpansionChipStatus expansionChipStatus;
    private BoxCollider2D boxCollider2D;

    private float horizontal;
    private float moveDirection;
    [HideInInspector] public float movespeed;
    private float jumpingPower;
    private float gravityScale;
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

    private float HoldPositionDelay;

    public bool blocked = false;

    #region Wall Slide & Wall Jump
    [Header("Wall Slide & Wall Jump")]
    public bool wallJumpUnlocked = false;
    private bool isWallSliding;
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
    private float dashTime = 0.1f;
    private float dashCooldown = 1f;
    private float dashCurrentCooldown;
    #endregion
    
    private float _fallSpeedYDampingChangeThreshold;

    [HideInInspector] public Rigidbody2D rb;
    private Animator anim;
    private CharacterStats stats;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] public LayerMask wallLayer;

    // Only for Debugging
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
        anim = GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();

        movespeed = stats.MoveSpeed;
        jumpingPower = stats.JumpForce;
        gravityScale = stats.GravityScale;
        rb.gravityScale = gravityScale;
        extraJumpsCounter = extraJumps;
        coyoteTimeCounter = coyoteTime;
        dashForce = stats.DashForce;
    }

    private void Start()
    {     
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    private void Update()
    {
        if (blocked || isDashing) return;
        horizontal = playerInput.Player.Move.ReadValue<Vector2>().x;
        if(horizontal < 0)
        {
            moveDirection = -1f;
        }
        else if(horizontal > 0)
        {
            moveDirection = 1f;
        }
        else
        {
            moveDirection = 0f;
        }
        #region Vertical Jumping
        if(playerInput.Player.Jump.triggered && !isWallSliding)
        {
            if(coyoteTimeCounter <= 0f)
            {
                if(expansionChipStatus.isDreamBuilderAvailable)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                    Instantiate(dreamBuilderPlatform, dreamBuilderPlatformSpawnPoint.position, Quaternion.identity);
                    expansionChipStatus.dreamBuilderPlatformCurrentCooldown = expansionChipStatus.dreamBuilderPlatformCooldown;
                }
                else if(extraJumpsCounter > 0)
                {
                    extraJumpsCounter--;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            }
        }
        if(playerInput.Player.Jump.WasReleasedThisFrame() && rb.linearVelocity.y > 0)
        {
            // Stop Moving Upwards immediately when Jump Button is released
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
            coyoteTimeCounter = -1f;
        }
        #endregion

        if(IsGrounded())
        {
            extraJumpsCounter  = extraJumps;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if(rb.linearVelocity.y < -20f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -20f);
        }


        WallSlide();
        WallJump();
        if(!isWallSliding)
        {
            HoldPositionDelay = 0.1f;
        }

        if (!isWallJumping)
        {
            if (transform.localRotation.y < 0 && moveDirection > 0f || transform.localRotation.y >= 0 && horizontal < 0f)
            {
                Flip();
            }
        }
        if (rb.linearVelocity.y < -_fallSpeedYDampingChangeThreshold)
        {
            CameraManager.instance.LowYDamping();
        }
        if(rb.linearVelocity.y >= 0f)
        {
            //Reset so it can be called again
            CameraManager.instance.NormalYDamping();
        }

        if(dashUnlocked && playerInput.Player.Dash.triggered && canDash)
        {
            StartCoroutine(Dash());
        }

        if(dashCurrentCooldown > 0f)
        {
            dashCurrentCooldown -= Time.deltaTime;
            canDash = false;
        }
        else
        {
            if(IsGrounded())
            {
                canDash = true;
                Debug.Log("Dash is Ready to Use when the player is on the ground"); 
            }
        }

        // Debug Only
        currentVelocityY = rb.linearVelocity.y;
    }

    private void FixedUpdate()
    {
        if(isDashing)
        {
            return;
        } 
            
        if (!isWallJumping)
        {
            // Horizontal Movement
            if(!blocked)
            {
                rb.linearVelocity = new Vector2(moveDirection * movespeed, rb.linearVelocity.y);
            }
        }
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityScale * 1.5f;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        #region Animation State
        anim.SetBool("isGrounded", IsGrounded());
        anim.SetBool("isJumping", rb.linearVelocity.y > 0f);
        anim.SetBool("isFalling", rb.linearVelocity.y < 0f);
        anim.SetBool("isRunning", rb.linearVelocity.x != 0f);
        anim.SetBool("IsWallSliding", isWallSliding);
        if (transform.localRotation.y < 0)
        {
            isFacingRight = false;
        }
        else if (transform.localRotation.y >= 0)
        {
            isFacingRight = true;
        }
        #endregion
    }

    #region Is Grounded
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);
        // Vector2 boxCastSize = new Vector2(boxCollider2D.bounds.size.x, 0.3f);
        // RaycastHit2D hit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCastSize, 0f, Vector2.down, 0.1f, groundLayer);
        // return hit.collider != null;
    }
    #endregion

    #region WallSlide & WallJump
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer) && !IsGrounded();
    }

    private void WallSlide()
    {
        if(!wallJumpUnlocked) return; //Skip Wall Slide if Wall Jump is not Unlocked

        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;

            if(extraJumpsCounter == 0)
            {
                extraJumpsCounter = extraJumps;
            }

            if(HoldPositionDelay >= 0f)
            {
                HoldPositionDelay -= Time.deltaTime;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if(!wallJumpUnlocked) return; //Skip Wall Jump if Wall Jump is not Unlocked
        if (isWallSliding)
        {
            isWallJumping = false;
            // Wall Jumping Direction
            if (transform.localRotation.y >= 0)
            {
                wallJumpingDirection = -1f;
            }
            else if (transform.localRotation.y < 0)
            {
                wallJumpingDirection = 1f;
            }
            else
            {
                Debug.LogError("WallJumpingDirection Error");
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
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            transform.Rotate(0f, 180f, 0f);

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
        trailRenderer.emitting = false;
    }
    #endregion

    
    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
    }
    #region Dash
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        if(extraJumpsCounter == 0)
        {
            extraJumpsCounter = extraJumps;
        }

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection;
        if(horizontal < 0)
        {
            dashDirection = -1f;
        }
        else if(horizontal > 0)
        {
            dashDirection = 1f;
        }
        else
        {
            if(IsGrounded() && rb.linearVelocity.x == 0 )
            {
                dashDirection = isFacingRight ? -1f : 1f;
            }
            else
            {
                dashDirection = isFacingRight ? 1f : -1f;
            }
        }
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        trailRenderer.emitting = true;
        if (transform.localRotation.y < 0 && dashDirection > 0f || transform.localRotation.y >= 0 && dashDirection < 0f)
        {
            Flip();
        }
        yield return new WaitForSeconds(dashTime);
        trailRenderer.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        dashCurrentCooldown = dashCooldown;
    }
    #endregion


    public void BlockMovement()
    {
        rb.linearVelocity = Vector2.zero;
        blocked = true;
    }
    public void EnableMovement()
    {
        blocked = false;
    }


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