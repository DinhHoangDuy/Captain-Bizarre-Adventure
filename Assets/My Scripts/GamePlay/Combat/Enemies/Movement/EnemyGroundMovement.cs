using UnityEngine;

public class EnemyGroundMovement : MonoBehaviour
{
    private Animator anim;
    internal Rigidbody2D rb;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float movementRoutineTime = 2.5f;

    [SerializeField] private GameObject cliffCheck;
    [SerializeField] private GameObject wallCheck;
    [SerializeField] private LayerMask groundLayer;

    private BoxCollider2D enemyFrontSight;
    private BoxCollider2D enemyBackSight;

    private float movementRoutineTimer;
    internal bool isLookingRight = true;
    internal bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        movementRoutineTimer = movementRoutineTime;

        enemyFrontSight = gameObject.transform.Find("EnemyFrontSight").GetComponent<BoxCollider2D>();
        enemyBackSight = gameObject.transform.Find("EnemyBackSight").GetComponent<BoxCollider2D>();
        if (enemyFrontSight == null || enemyBackSight == null)
        {
            Debug.LogError("Enemy Front Sight or Enemy Back Sight is not found!");
        }

        // Check if the Gameobject variables are assigned
        if (cliffCheck == null || wallCheck == null)
        {
            Debug.LogError("Cliff Check or Wall Check is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        int direction = isLookingRight ? 1 : -1;

        if (!EnemyFound())
        {
            Debug.Log("Enemy is not seeing the player. Wandering around.");
            if (movementRoutineTimer <= 0)
            {
                Flip();
            }
            else
            {
                movementRoutineTimer -= Time.deltaTime;
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocityY);
                MovingCheck();
            }
        }
        else if (EnemyFound())
        {
            if (isAttacking)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
            }
            else
            {
                if(IsEnemyBehind())
                {
                    Flip();
                }
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocityY);
            }
        }

        // Update Animation
        anim.SetBool("isWalking", rb.linearVelocityX != 0);
    }

    void Flip()
    {
        isLookingRight = !isLookingRight;
        transform.Rotate(0f, 180f, 0f);
        movementRoutineTimer = movementRoutineTime;
    }

    void MovingCheck()
    {
        if (!Physics2D.OverlapCircle(cliffCheck.transform.position, 0.2f, groundLayer))
        {
            Flip();
            Debug.Log("Cliff detected. Turning around.");
        }
        if (Physics2D.OverlapCircle(wallCheck.transform.position, 0.2f, groundLayer))
        {
            Flip();
            Debug.Log("Wall detected. Turning around.");
        }
    }
    internal bool EnemyFound()
    {
        return enemyFrontSight.IsTouchingLayers(LayerMask.GetMask("Player")) || enemyBackSight.IsTouchingLayers(LayerMask.GetMask("Player"));
    }
    private bool IsEnemyBehind()
    {
        return enemyBackSight.IsTouchingLayers(LayerMask.GetMask("Player"));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(cliffCheck.transform.position, 0.1f);
    }
}
