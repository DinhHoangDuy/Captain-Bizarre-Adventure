using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyResistance))]
public class EnemyHealth : MonoBehaviour
{
    private EnemyResistance enemyResistance;
    [SerializeField] private float maxHealth;
    private float currentHealth;

    /// <summary>
    /// Initializes the enemy's current health to the maximum health value.
    /// </summary>
    private void Start()
    {
        enemyResistance = GetComponent<EnemyResistance>();
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Reduces the enemy's current health by the specified damage amount.
    /// </summary>
    /// <param name="damage">The amount of damage to be taken.</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            GetComponent<Animator>().Play("Death");
        }
        else
        {
            GetComponent<Animator>().Play("Take Hit");
        }
    }

    /// <summary>
    /// Performs the death logic for the enemy and destroys the game object.
    /// </summary>
    public void Die()
    {
        // Add death logic here
        Destroy(gameObject);
    }
}
