using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Mushroom : MonoBehaviour, GroundEnemyBase
{
    [Header("Mushroom Properties")]
    [SerializeField] private float speed = 3f;

    [Tooltip("How long should this entity move in one direction?")]
    [SerializeField] private float movementRoutineTime = 4f;
    private float movementRoutineTimer = 0f;

    [Header("Mushroom Attack Properties")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint0;
    [SerializeField] private Transform projectileSpawnPoint1;
    [SerializeField] private Transform projectileSpawnPoint2;
    public float attackDelay = 2.5f;
    private float attackTimer = 0f;


    [Header("Mushroom Components")]
    [SerializeField] private BoxCollider2D enemyFrontSight;
    [SerializeField] private BoxCollider2D enemyBackSight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private GameObject CliffCheck;
    public Rigidbody2D rb { get; set; }
    private Animator anim;

    private bool playerFound = false;
    private bool isLookingRight = true;
    private bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        #region Movement
        if(playerFound)
        {
            // TODO - Make the mushroom shoot projectiles towards the player.
            Attack();
        }
        else Wander();
        #endregion

        #region Player Detection
        // Detech if the player is in the enemy sight (front or back)
        if(enemyFrontSight.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            playerFound = true;
        }
        else if(enemyBackSight.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            // If the player is behind the enemy, flip the enemy to face the player
            Flip();
            playerFound = true;
        }
        else playerFound = false;
        #endregion


        UpdateAnimation();
    }

    public void Wander()
    {
        // If the entity is attacking, stop moving
        if(isAttacking) return;
        
        // Attack Logic
        if(movementRoutineTimer <= 0)
        {
            Flip();
        }
        else
        {
            movementRoutineTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(speed * (isLookingRight ? 1 : -1), rb.linearVelocity.y);
            MovingCheck();
        }
    }

    public void MovingCheck()
    {
        // Check if the entity is about to hit a wall
        if(Physics2D.OverlapCircle(wallCheck.transform.position, 0.2f, wallLayer))
        {
            Flip();
        }

        // Check if the entity is about to fall off a cliff
        if(!Physics2D.OverlapCircle(CliffCheck.transform.position, 0.2f, groundLayer))
        {
            Flip();
        }
    }

    public void Attack()
    {
        // TODO - Make the mushroom shoot projectiles towards the player.
        if(attackTimer <= 0) // Attack when the timer is up
        {
            anim.SetTrigger("Attack");
            attackTimer = attackDelay;
        }
        else // Chase the player if the timer is not up, but the player is still in sight (only when it's not attacking)
        {
            if(!isAttacking)
            {
                rb.linearVelocity = new Vector2(speed * (isLookingRight ? 1 : -1), rb.linearVelocity.y);
                MovingCheck();
            }
            attackTimer -= Time.deltaTime;
        }
    }

    public void Flip()
    {
        if(isAttacking) return;
        
        isLookingRight = !isLookingRight;
        transform.Rotate(0f, 180f, 0f);
        movementRoutineTimer = movementRoutineTime;
    }

    // Update the animation based on the current state of the entity
    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", rb.linearVelocity.x != 0);
    }
    public void StopMoving()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isAttacking = true;
    }
    public void ResumeMoving()
    {
        isAttacking = false;
    }

    // Animation Event
    public void ShootProjectile()
    {
        int throwDirection = isLookingRight ? 1 : -1;
        ShootProjectileFromOnePoint(projectileSpawnPoint0, new Vector2(throwDirection * 10, 5f), 1.5f);
        ShootProjectileFromOnePoint(projectileSpawnPoint1, new Vector2(throwDirection * 10, 7f), 1.5f);
        ShootProjectileFromOnePoint(projectileSpawnPoint2, new Vector2(throwDirection * 10, 9f), 1.5f);
    }
    private void ShootProjectileFromOnePoint(Transform spawnPoint, Vector2 throwPower, float gravitiyScale)
    {
        //====== Instantiate the projectile ======//
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        //====== Set the projectile properties ======//
        projectile.transform.Rotate(0f, isLookingRight ? 0f : 180f, 0f);
        projectile.GetComponent<EnemyProjectile>().SetProjectileDamage(damage);
        projectile.GetComponent<EnemyProjectile>().SetProjectileType(ProjectileType.Curve);
        projectile.GetComponent<EnemyProjectile>().SetProjectileGravityScale(gravitiyScale);

        //====== Throw the projectile ======//
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = throwPower;
    }

    // Debugging
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(enemyFrontSight.bounds.center, enemyFrontSight.bounds.size);
        Gizmos.DrawWireCube(enemyBackSight.bounds.center, enemyBackSight.bounds.size);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheck.transform.position, 0.2f);
        Gizmos.DrawWireSphere(CliffCheck.transform.position, 0.2f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(projectileSpawnPoint0.position, 0.2f);
        Gizmos.DrawWireSphere(projectileSpawnPoint1.position, 0.2f);
        Gizmos.DrawWireSphere(projectileSpawnPoint2.position, 0.2f);
    }
}
