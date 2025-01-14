using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyResistance))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    private Rigidbody2D rb;

    [Header("Enemy Health Settings")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float maxHealth;

    [Header("Developer Settings")]
    [Tooltip("If the enemt is a dummy object, it will not take damage. Their damage taken will be recorded instead.")]
    [SerializeField] internal bool isDummy = false;
    public float damageTaken = 0;
    public bool invincibleAlwaysOn = false;
    public bool unableToPush = false;
    internal bool isDead = false;

    private float force;
    private int pushDirection;
    private float currentHealth;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (enemyType == EnemyType.Destroyable)
        {
            return;
        }

        if (isDummy)
        {
            // Record the damage taken by the dummy object.
            damageTaken += damage;
            Debug.Log("Dummy object took " + damage + " damage. Total damage taken: " + damageTaken);
            return;
        }
        else
        {
            // Calculate the damage taken by the enemy. Ignore the resistance if the enemy is invincible.
            if (!invincibleAlwaysOn)
            {
                currentHealth -= damage;
                Debug.Log("Enemy took " + damage + " damage. Current Health: " + currentHealth);
            }
            else
            {
                Debug.Log("Enemy is invincible and cannot take damage");
            }
        }

        if (currentHealth <= 0)
        {
            // Run death animation
            Die();
        }
        else
        {
            // Push the enemy back
            if (!unableToPush)
            {
                Vector2 pushDirection = new Vector2(this.pushDirection * force, 0);
                rb.AddForce(pushDirection, ForceMode2D.Impulse);
            }
        }
    }
    public void DestroyableTakeDMG(int damage)
    {
        if (enemyType != EnemyType.Destroyable)
        {
            return;
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Performs the death logic for the enemy and destroys the game object.
    /// </summary>
    private void Die()
    {
        isDead = true;
        GetComponent<Animator>().SetTrigger("Die");
    }

    // If the enemy is an animating creature (e.g. a slime), the death animation will call this function to DEACTIVATE the game object.
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
    // If the enemy is a destroyable object (e.g. a crate), the death animation will call this function to DESTROY the game object.
    public void Destroy()
    {
        Destroy(gameObject);
    }
    // If the enemy is a boss, which can only be defeated once, the death animation will call this function to DEACTIVATE the game object.
    // TODO: Add the logic to prevent the boss from being reactivated.

}

enum EnemyType
{
    Normal,
    Boss,
    Destroyable
}
