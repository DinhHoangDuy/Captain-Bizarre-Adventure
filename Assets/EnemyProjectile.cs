using UnityEditor.Callbacks;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public ProjectileType projectileType;

    private int projectileDamage = 1;

    // Use these variables to set the projectile speed if the projectile type is set to straght.
    private float projectileStraightSpeed;
    // Use these variables to set the projectile speed if the projectile type is set to curve.
    private float desiredGravityScale;

    public void SetProjectileType(ProjectileType type)
    {
        projectileType = type;
    }
    public void SetProjectileGravityScale(float gravityScale)
    {
        desiredGravityScale = gravityScale;
    }
    public void SetProjectileDamage(int damage)
    {
        projectileDamage = damage;
    }

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (projectileType == ProjectileType.Straight)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = transform.right * projectileStraightSpeed;
        }
        else if (projectileType == ProjectileType.Curve)
        {
            rb.gravityScale = desiredGravityScale;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Hit the player!");
            other.GetComponent<PlayerHealth>().TakeDamage(projectileDamage);
            Destroy(gameObject);
        }
        
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Debug.Log("Hit the ground!");
            Destroy(gameObject);
        }
    }
}

public enum ProjectileType
{
    Curve,
    Straight,
}
