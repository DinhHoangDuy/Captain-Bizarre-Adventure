using UnityEngine;

public interface GroundEnemyBase
{
    public abstract Rigidbody2D rb { get; set; }
    public abstract void Wander();
    // Copy these lines to any class that implements the GroundEnemyBase interface.
    // public void Wander()
    // {
    //     attackTimer -= Time.deltaTime;
    //     if(movementRoutineTimer <= 0)
    //     {
    //         movementRoutineTimer = movementRoutineTime;
    //         Flip();
    //     }
    //     else
    //     {
    //         movementRoutineTimer -= Time.deltaTime;
    //         rb.linearVelocity = new Vector2(speed * (isLookingRight ? 1 : -1), rb.linearVelocity.y);
    //     }
    // }

    public abstract void Attack();
    // Copy these lines to any class that implements the GroundEnemyBase interface.
    // public void Attack()
    // {
    //     // TODO - Make the mushroom shoot projectiles towards the player.
    //     if(attackTimer <= 0)
    //     {
                //=== This is just a placeholder for the actual attack code ===
    //         // GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
    //         // projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed * (isLookingRight ? 1 : -1), 0f);
    //         Debug.Log("Mushroom says: Kaboom!");
    //         attackTimer = attackDelay;
    //     }
    //     else 
    //     {
    //         Wander();
    //         attackTimer -= Time.deltaTime;
    //     }
    // }
}
