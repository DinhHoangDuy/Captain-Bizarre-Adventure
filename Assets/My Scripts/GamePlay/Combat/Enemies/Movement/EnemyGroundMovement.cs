using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyGroundMovement : MonoBehaviour
{
    private Animator anim;
    internal Rigidbody2D rb;
    private EnemyHealth enemyHealth;

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
        enemyHealth = GetComponent<EnemyHealth>();

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
        if (GetComponent<EnemyHealth>().isDead) return;

        int direction = isLookingRight ? 1 : -1;
        anim.SetBool("isWalking", rb.linearVelocityX != 0);
        if (enemyHealth.isDummy)
        {
            return; // If the enemy is a dummy, it will not move.
        }

        if (!EnemyFound())
        {
            // Debug.Log("Enemy is not seeing the player. Wandering around.");
            if (movementRoutineTimer <= 0)
            {
                Flip();
                movementRoutineTimer = movementRoutineTime;
                // StartCoroutine(PauseBeforeTurning());
            }
            else
            {
                movementRoutineTimer -= Time.deltaTime;
                // rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocityY);
                float targetSpeed = direction * speed;
                MoveWithForce(targetSpeed);
                // rb.AddForce(new Vector2(direction * speed, 0), ForceMode2D.Force);
                MovingCheck();
            }
        }
        else if (EnemyFound())
        {
            // if (isAttacking)
            // {
            //     rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
            // }
            // else
            // {
            //     if (IsEnemyBehind())
            //     {
            //         Flip();
            //     }
            //     // rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocityY);
            //     float targetSpeed = direction * speed;
            //     MoveWithForce(targetSpeed);
            // }
            if (IsEnemyBehind())
            {
                Flip();
            }

            float targetSpeed = direction * speed;
            MoveWithForce(targetSpeed);
        }

        // Update Animation
    }

    void Flip()
    {
        isLookingRight = !isLookingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void MoveWithForce(float targetSpeed)
    {
        // Set desired speed (velocity) based on input.
        // float targetSpeed = moveDirection * moveSpeed;
        // Calculate the difference between current velocity and target velocity.
        float speedChange = targetSpeed - rb.linearVelocityX;
        // Change acceleration based on the difference.
        float accelRate = (Mathf.Abs(speedChange) > 0.1f) ? 10 : -10;

        float movement = Mathf.Pow(Mathf.Abs(speedChange) * accelRate, 1) * Mathf.Sign(speedChange);

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    void MovingCheck()
    {
        if (!Physics2D.OverlapCircle(cliffCheck.transform.position, 0.2f, groundLayer))
        {
            Flip();
            movementRoutineTimer = movementRoutineTime;
            // Debug.Log("Cliff detected. Turning around.");
            // StartCoroutine(PauseBeforeTurning());
        }
        if (Physics2D.OverlapCircle(wallCheck.transform.position, 0.2f, groundLayer))
        {
            Flip();
            movementRoutineTimer = movementRoutineTime;
            // Debug.Log("Wall detected. Turning around.");
            // StartCoroutine(PauseBeforeTurning());
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

    private IEnumerator PauseBeforeTurning()
    {
        // Pause for a bit before turning around
        yield return new WaitForSeconds(1.0f); // Adjust the wait time as needed

        Flip();
        movementRoutineTimer = movementRoutineTime;
        // Debug.Log("Turning around after pause.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(cliffCheck.transform.position, 0.1f);
    }
}
