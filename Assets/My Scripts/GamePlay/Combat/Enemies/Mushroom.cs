using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyGroundMovement))]
public class Mushroom : MonoBehaviour
{
    private Animator anim;
    private EnemyHealth enemyHealth;
    private EnemyGroundMovement enemyGroundMovement;

    [SerializeField] private int attackDamage = 1;
    [SerializeField] private GameObject enemyProjectile;
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private float projectileGravityScale = 1.5f;
    [SerializeField] private Transform projectileSpawnPoint1;
    [SerializeField] private Vector2 throwPowerFromSpawnPoint1;
    [SerializeField] private Transform projectileSpawnPoint2;
    [SerializeField] private Vector2 throwPowerFromSpawnPoint2;
    [SerializeField] private Transform projectileSpawnPoint3;
    [SerializeField] private Vector2 throwPowerFromSpawnPoint3;


    private float attackCooldown = 2.5f;
    private float attackTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyGroundMovement = GetComponent<EnemyGroundMovement>();
    }
    void Update()
    {
        if (!enemyGroundMovement.EnemyFound()) return;
        if (attackTimer <= 0 && !enemyGroundMovement.isAttacking)
        {
            anim.SetTrigger("Attack");
            attackTimer = attackCooldown;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }        
    }

    // Animation events
    public void StopMoving()
    {
        enemyGroundMovement.rb.linearVelocity = new Vector2(0f, enemyGroundMovement.rb.linearVelocity.y);
        enemyGroundMovement.isAttacking = true;
    }
    public void ResumeMoving()
    {
        enemyGroundMovement.isAttacking = false;
    }
    public void ShootProjectile()
    {
        int throwDirection = enemyGroundMovement.isLookingRight ? 1 : -1;
        ShootProjectileFromOnePoint(projectileSpawnPoint1, new Vector2(throwDirection * throwPowerFromSpawnPoint1.x, 5f), 1.5f);
        ShootProjectileFromOnePoint(projectileSpawnPoint2, new Vector2(throwDirection * throwPowerFromSpawnPoint2.x, 7f), 1.5f);
        ShootProjectileFromOnePoint(projectileSpawnPoint3, new Vector2(throwDirection * throwPowerFromSpawnPoint3.x, 9f), 1.5f);
        // ShootProjectileFromOnePoint(projectileSpawnPoint1, throwDirection * throwPowerFromSpawnPoint1, projectileGravityScale);
        // ShootProjectileFromOnePoint(projectileSpawnPoint2, throwDirection * throwPowerFromSpawnPoint2, projectileGravityScale);
        // ShootProjectileFromOnePoint(projectileSpawnPoint3, throwDirection * throwPowerFromSpawnPoint3, projectileGravityScale);
    }
    public void ShootProjectileFromOnePoint(Transform spawnPoint, Vector2 throwPower, float gravitiyScale)
    {
        //====== Instantiate the projectile ======//
        GameObject projectile = Instantiate(enemyProjectile, spawnPoint.position, Quaternion.identity);

        //====== Set the projectile properties ======//
        projectile.transform.Rotate(0f, enemyGroundMovement.isLookingRight ? 0f : 180f, 0f);
        projectile.GetComponent<EnemyProjectile>().SetProjectileDamage(attackDamage);
        projectile.GetComponent<EnemyProjectile>().SetProjectileType(ProjectileType.Curve);
        projectile.GetComponent<EnemyProjectile>().SetProjectileGravityScale(gravitiyScale);

        //====== Throw the projectile ======//
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = throwPower;
    }
    
    // Debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(projectileSpawnPoint1.position, 0.5f);
        Gizmos.DrawWireSphere(projectileSpawnPoint2.position, 0.5f);
        Gizmos.DrawWireSphere(projectileSpawnPoint3.position, 0.5f);
    }
}