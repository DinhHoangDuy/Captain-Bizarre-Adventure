using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyResistance))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    private EnemyResistance enemyResistance;
    [SerializeField] private float maxHealth;
    [SerializeField] private bool isDummy = false;
    private float force;
    private int pushDirection;
    private float currentHealth;
    
    [Header("Developer Settings")]
    public bool invincibleAlwaysOn = false;
    public bool unableToPush = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

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
        if (invincibleAlwaysOn)
        {
            Debug.Log("Enemy is invincible and cannot take damage");
            Vector2 pushDirection = new Vector2(this.pushDirection * force, 0);
            rb.AddForce(pushDirection, ForceMode2D.Impulse);
            return;
        }

        // Check if the GameObject has an Dummy script attached to it, if yes, sent the damage value to the script instead of reducing Health point.
        if (GetComponent<Dummy>() == null && isDummy)
        {
            GetComponent<Dummy>().TakeDamage(damage);
        }        
        else
        {
            currentHealth -= damage;
            Debug.Log("Enemy took " + damage + " damage. Current Health: " + currentHealth);
        }       

        if (currentHealth <= 0)
        {
            // TODO: add death animation.
            // GetComponent<Animator>().Play("Death");
            Die();
        }
        else
        {
            Vector2 pushDirection = new Vector2(this.pushDirection * force, 0);
            rb.AddForce(pushDirection, ForceMode2D.Impulse);
        }
    }
    public void DestroyableTakeDMG(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Performs the death logic for the enemy and destroys the game object.
    /// </summary>
    public void Die()
    {
        // Add death logic here
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
