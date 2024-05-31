using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyResistance))]
public class EnemyHealth : MonoBehaviour
{
    private EnemyResistance enemyResistance;
    [SerializeField] private float maxHealth;
    private float currentHealth;

    private void Start()
    {
        enemyResistance = GetComponent<EnemyResistance>();
        //This line of code sets the current health of the enemy to the maximum health value, which is set in the EnemyResistance script.
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death logic here
        Destroy(gameObject);
    }
}
