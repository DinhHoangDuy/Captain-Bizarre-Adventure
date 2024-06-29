using System.Collections;
using UnityEngine;

public class PlatformerMovement2D : MonoBehaviour
{
    public static PlatformerMovement2D instance;
    private PlayerInput playerInput;
    private ExpansionChipStatus expansionChipStatus;

    private float horizontal;
    public float movespeed;
    private float jumpingPower;
    private float gravityScale;
    private int extraJumps;
    private int extraJumpsCounter;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    #region Dream Builder Chip
    [Header("Dream Builder Chip")]
    [SerializeField] private GameObject dreamBuilderPlatform;
    [SerializeField] private Transform dreamBuilderPlatformSpawnPoint;
    #endregion

    private bool isFacingRight = true;
    public bool IsLookingRight => isFacingRight;
    public bool blocked = false;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(6f, 16f);

    private bool canDash = true;
    private bool isDashing = false;
    private float dashForce;
    private float dashTime = 0.2f;
    private float dashCooldown = 1f;
    [SerializeField] private TrailRenderer trailRenderer;


    private float _fallSpeedYDampingChangeThreshold;

    [HideInInspector] public Rigidbody2D rb;
    private Animator anim;
    private CharacterStats stats;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    // Only for Debugging
    private float currentVelocityY;

    private void OnEnable()
    {
        playerInput.Game.Enable();
    }
    private void OnDisable()
    {
        playerInput.Game.Disable();
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
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();

        movespeed = stats.MoveSpeed;
        jumpingPower = stats.JumpForce;
        gravityScale = stats.GravityScale;
        rb.gravityScale = gravityScale;
        extraJumps = stats.ExtraJumps;
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
        horizontal = playerInput.Game.WASD.ReadValue<Vector2>().x;

        #region Vertical Jumping
        // if (playerInput.Game.Jump.triggered && (coyoteTimeCounter > 0 || extraJumpsCounter  > 0))
        // {
        //     if(!IsGrounded() && coyoteTimeCounter <= 0 && !isWallSliding)
        //     // if(!IsGrounded() && coyoteTimeCounter <= 0)
        //     {
        //         if(expansionChipStatus.isDreamBuilderAvailable)
        //         {
        //             Instantiate(dreamBuilderPlatform, dreamBuilderPlatformSpawnPoint.position, Quaternion.identity);
        //             expansionChipStatus.dreamBuilderPlatformCurrentCooldown = expansionChipStatus.dreamBuilderPlatformCooldown;
        //         }
        //         else
        //         {
        //             extraJumpsCounter --;
        //         }
        //     }

        //     rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        // }
        if(playerInput.Game.Jump.triggered && !isWallSliding)
        {
            if(coyoteTimeCounter <= 0)
            {
                if(expansionChipStatus.isDreamBuilderAvailable)
                {
                    Instantiate(dreamBuilderPlatform, dreamBuilderPlatformSpawnPoint.position, Quaternion.identity);
                    expansionChipStatus.dreamBuilderPlatformCurrentCooldown = expansionChipStatus.dreamBuilderPlatformCooldown;
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                }
                else if(extraJumpsCounter > 0)
                {
                    extraJumpsCounter --;
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                }
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
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

        if(rb.velocity.y < -20f)
        {
            rb.velocity = new Vector2(rb.velocity.x, -20f);
        }


        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            if (transform.localRotation.y < 0 && horizontal > 0f || transform.localRotation.y >= 0 && horizontal < 0f)
            {
                Flip();
            }
        }
        if (rb.velocity.y < -_fallSpeedYDampingChangeThreshold)
        {
            CameraManager.instance.LowYDamping();
        }
        if(rb.velocity.y >= 0f)
        {
            //Reset so it can be called again
            CameraManager.instance.NormalYDamping();
        }

        if(playerInput.Game.Dash.triggered && canDash)
        {
            StartCoroutine(Dash());
        }

        // Debub Only
        currentVelocityY = rb.velocity.y;

    }

    private void FixedUpdate()
    {
        if(isDashing)
        {
            return;
        } 
            
        if (!isWallJumping)
        {
            if(!blocked)
            {
                rb.velocity = new Vector2(horizontal * movespeed, rb.velocity.y);
            }
        }
        if(rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * 1.5f;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        #region Animation State
        anim.SetBool("isGrounded", IsGrounded());
        anim.SetBool("isJumping", rb.velocity.y > 0f);
        anim.SetBool("isFalling", rb.velocity.y < 0f);
        anim.SetBool("isRunning", rb.velocity.x != 0f);
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, groundLayer);
        // TODO: Check if the raycast hit the ground layer or the DreamGround layer
        // Debug.Log("Raycast hit layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
        return hit.collider != null;
        // return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    #endregion

    #region WallSlide & WallJump
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer) && !IsGrounded();
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
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
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (playerInput.Game.Jump.triggered && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            transform.Rotate(0f, 180f, 0f);

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
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
            if(IsGrounded() && rb.velocity.x == 0 )
            {
                dashDirection = isFacingRight ? -1f : 1f;
            }
            else
            {
                dashDirection = isFacingRight ? 1f : -1f;
            }
        }
        rb.velocity = new Vector2(dashDirection * dashForce, 0f);

        trailRenderer.emitting = true;
        if (transform.localRotation.y < 0 && dashDirection > 0f || transform.localRotation.y >= 0 && dashDirection < 0f)
        {
            Flip();
        }
        yield return new WaitForSeconds(dashTime);
        trailRenderer.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion


    public void BlockMovement()
    {
        rb.velocity = Vector2.zero;
        blocked = true;
    }
    public void EnableMovement()
    {
        blocked = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);
    }
}