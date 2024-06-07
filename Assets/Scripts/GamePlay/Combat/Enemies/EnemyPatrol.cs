using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    // Variables
    [SerializeField] private Transform patrolPoint1;
    [SerializeField] private Transform patrolPoint2;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseRange = 5f;

    // Components
    private Transform currentPatrolPoint;
    private Transform player;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private Animator animator;
    private Vector3 target;
    private bool isChasing = false;
    private bool isWaiting = false;
    private bool isRunning = false;
    private bool isAttacking = false;
    private float threshold = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = patrolPoint1.position;
        currentPatrolPoint = patrolPoint1;
    }

    void Update()
    {
        if(isAttacking)
        {
            return;
        }
        
        // Calculate distance and direction to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 enemyForward = new Vector2(transform.localScale.x, 0).normalized;


        Patrol();
        if (distanceToPlayer < chaseRange && Vector2.Dot(directionToPlayer, enemyForward) > 0 && player.position.y >= transform.position.y - threshold || isChasing && player.position.y >= transform.position.y - threshold)
        {
            isChasing = true;
            target = player.position;
            Debug.Log("Chasing player");
        }
        else
        {
            isChasing = false;
            target = currentPatrolPoint.position;
            Debug.Log("Patrolling to " + target);
        }

        if (!isWaiting || isChasing)
        {
            MoveTowardsTarget();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        // Flip the enemy sprite based on the direction it is moving
        if(target.x > transform.position.x && !isWaiting)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if(target.x < transform.position.x && !isWaiting)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        animator.SetBool("isRunning", rb.velocity.magnitude > 0f);
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, patrolPoint1.position) <= 0.2f && currentPatrolPoint == patrolPoint1)
        {
            Debug.Log("Reached patrol point 1, switching to patrol point 2");
            currentPatrolPoint = patrolPoint2;
            StartCoroutine(StandStill());
        }
        else if (Vector2.Distance(transform.position, patrolPoint2.position) <= 0.2f && currentPatrolPoint == patrolPoint2)
        {
            Debug.Log("Reached patrol point 2, switching to patrol point 1");
            currentPatrolPoint = patrolPoint1;
            StartCoroutine(StandStill());
        }
    }
    IEnumerator StandStill()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
    }

    void MoveTowardsTarget()
    {
        if (Vector2.Distance(transform.position, currentPatrolPoint.position) > 0.2f || isChasing)
        {
            float step = (isChasing ? chaseSpeed : speed) * Time.deltaTime;
            Vector2 oldPosition = transform.position;
            Vector2 targetPosition = new Vector2(target.x, transform.position.y); // Only change x-coordinate
            Vector2 newPosition = Vector2.MoveTowards(oldPosition, targetPosition, step);

            // Calculate velocity
            velocity = (newPosition - oldPosition) / Time.deltaTime;

            // Move the enemy
            rb.MovePosition(newPosition);

            // Update the Rigidbody2D's velocity
            rb.velocity = velocity;
        }
    }

    public void IsAttacking()
    {
        isAttacking = true;
    }
    public void IsNotAttacking()
    {
        isAttacking = false;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(patrolPoint1.position, patrolPoint2.position);
        Gizmos.DrawWireSphere(patrolPoint1.position, 0.2f);
        Gizmos.DrawWireSphere(patrolPoint2.position, 0.2f);
    }
}
