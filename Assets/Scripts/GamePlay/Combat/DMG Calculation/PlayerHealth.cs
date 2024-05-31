using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TakeDMG))]
public class PlayerHealth : MonoBehaviour
{
    private int maxHealth;
    private CharacterStats characterStats;
    private Rigidbody2D rb2d;
    public int _maxHealth { get { return maxHealth; } }
    public int currentHealth { get; private set; }

    [HideInInspector] public bool characterHit = false;
    public bool isDead { get { return currentHealth <= 0; } }
    private bool isInvincible = false;
    private int invincibilityTime = 1;
    
    [Header("Player Health Settings")]
    [SerializeField] private float knockbackForce = 5.0f;

    // Respawn the player at the last checkpoint
    private Transform lastCheckpoint;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        characterStats = GetComponent<CharacterStats>();
        maxHealth = characterStats._maxHealth;
    }

    private void Start()
    {
        // Defensive programming to make sure the max health is not 0 or less than 0
        if(maxHealth <= 0)
        {
            Debug.LogError("Max Health cannot be 0 or less than 0!!");
            return;
        }
        // Set the current health to the max health
        currentHealth = maxHealth;

        // Get the TakeDMG script attached to the same GameObject
        TakeDMG TakeDMGScript = GetComponent<TakeDMG>();
        if (TakeDMGScript != null)
        {
            TakeDMGScript.OnHitPlayerReceived += TakeDamage;
        }

        // Set the last checkpoint to the current position of the player
        lastCheckpoint = transform;
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
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        characterHit = true;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player is Dead");
            Respawn();
        }
        else
        {
            // Calculate the direction from the damage source to the player
            Vector2 knockbackDirection = (Vector2)transform.position - damageSourcePosition;
            knockbackDirection.Normalize();

            // Apply a force to the player's Rigidbody2D component in the opposite direction of the damage source
            GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

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
            lastCheckpoint = collision.transform;
            Debug.Log("Checkpoint Reached!: " + lastCheckpoint.position);
        }
    }
    private void Respawn()
    {
        // Respawn the player at the last checkpoint
        this.transform.position = lastCheckpoint.position;
        currentHealth = maxHealth;
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}