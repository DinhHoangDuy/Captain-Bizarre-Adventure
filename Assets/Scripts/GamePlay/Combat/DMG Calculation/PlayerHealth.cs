using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TakeDMG))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    private CharacterStats characterStats;
    private Rigidbody2D rb2d;
    public int currentHealth { get; private set; }

    [HideInInspector] public bool characterHit = false;
    public bool isDead { get { return currentHealth <= 0; } }
    private bool isInvincible = false;
    private int invincibilityTime = 1;
    
    [Header("Player Health Settings")]
    [SerializeField] private float knockbackForce = 5.0f;

    // Respawn the player at the last checkpoint
    private Vector2 lastCheckpoint;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        characterStats = GetComponent<CharacterStats>();
        maxHealth = characterStats.maxHealth;
    }

    private void Start()
    {
        // Set the current health to the max health
        currentHealth = maxHealth;
        // Defensive programming to make sure the max health is not 0 or less than 0
        if(maxHealth <= 0)
        {
            Debug.LogError("Max Health cannot be 0 or less than 0!!");
            return;
        }
        lastCheckpoint = transform.position;
        if(lastCheckpoint != null)
        {
            Debug.Log("Last Checkpoint is created when loading Level.: " + lastCheckpoint);
        }
        else
        {
            Debug.LogError("Failed to set the last checkpoint upon spawning!");
        }

        // Get the TakeDMG script attached to the same GameObject
        TakeDMG TakeDMGScript = GetComponent<TakeDMG>();
        if (TakeDMGScript != null)
        {
            TakeDMGScript.OnHitPlayerReceived += TakeDamage;
        }
    }


    private void TakeDamage(int damage, Vector2 damageSourcePosition)
    // private void TakeDamage(int damage)
    {
        if(isInvincible)
        {
            Debug.Log("Player is Invincible!");
            return;
        }
        // Clamp the current health to be between 0 and max health
        if(ExpansionChipStatus.instance.isOverclocked)
        { 
            // If the ExpansionChipStatuc is overclocked, the player will receive more damage
            currentHealth = Mathf.Clamp(currentHealth - damage - 1, 0, maxHealth);
        } 
        else currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        characterHit = true;
        
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player is Dead");
            Respawn();
        }
        else
        {
            GetComponent<Animator>().Play("Hit");
            // Calculate the direction from the damage source to the player
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 knockbackDirection = (Vector2)transform.position - damageSourcePosition;
            knockbackDirection.Normalize();

            // Apply a force to the player's Rigidbody2D component in the opposite direction of the damage source
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            // Add a constant force that always pulls the player behind when the vector x is less than 0.5 and greater than -0.5
            if(knockbackDirection.x < 0.5 && knockbackDirection.x > -0.5)
            {
                rb.AddForce(Vector2.left * knockbackForce, ForceMode2D.Impulse);
            }
            StartCoroutine(IFrame(invincibilityTime));
        }
    }
    public void SacrificiceHealth(int healthToSacrifice)
    {
        currentHealth = Mathf.Clamp(currentHealth - healthToSacrifice, 0, maxHealth);
    }
    public void IncreaseHealth(int healthToAdd)
    {
        currentHealth = Mathf.Clamp(currentHealth + healthToAdd, 0, maxHealth);
        Debug.Log("Player's Health increased by " + healthToAdd);
    }
    public void Invincible(int time)
    {
        StartCoroutine(IFrame(time));
    }

    private IEnumerator IFrame(int time)
    {
        isInvincible = true;
        Debug.Log("Player is Invincible for " + time + " second");

        // Assuming you have a reference to the SpriteRenderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Flicker for 1 second
        float endTime = Time.time + 1f;
        while (Time.time < endTime)
        {
            // Toggle visibility
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // Wait for a short period of time
            yield return new WaitForSeconds(0.1f);
        }

        // Ensure the sprite is enabled at the end
        spriteRenderer.enabled = true;
        characterHit = false;
        isInvincible = false;
        Debug.Log("Player is no longer Invincible! Be careful!");
    }

    // Check if the character hit the FallingZone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("FallingZone"))
        {
            isInvincible = false;
            TakeDamage(maxHealth, collision.transform.position);
        }

        if (collision.CompareTag("Checkpoint"))
        {
            lastCheckpoint = collision.transform.position;
            Debug.Log("Checkpoint Reached!: " + lastCheckpoint);
        }
    }
    private void Respawn()
    {
        // Respawn the player at the last checkpoint
        transform.position = lastCheckpoint;
        currentHealth = maxHealth;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}