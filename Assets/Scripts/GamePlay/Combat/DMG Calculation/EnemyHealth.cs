using System.Collections;
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

    // Being Pushed
    private float direction;
    private float force;
    public bool isPushed = false; 
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
    private void Update()
    {
        if(isPushed)
        {
            rb.AddForce(new Vector2(direction * force, 0), ForceMode2D.Impulse);
            return;
        }

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
            rb.AddForce(new Vector2(direction * force, 0), ForceMode2D.Impulse);
        }
    }
    public void SetPushDirectionAndPower(float direction, float force)
    {
        this.direction = direction;
        this.force = force;
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
