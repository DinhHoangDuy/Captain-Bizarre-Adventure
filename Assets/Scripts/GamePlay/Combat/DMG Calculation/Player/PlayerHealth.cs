using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public int maxHealth;
    private CharacterStats characterStats;
    private Rigidbody2D rb2d;
    public int currentHealth { get; private set; }

    public bool isDead { get { return currentHealth <= 0; } }
    private bool isInvincible = false;
    private int invincibilityTime = 1;
    
    [Header("Player Health Settings")]
    [SerializeField] private float knockbackForce = 5.0f;

    // Respawn the player at the last checkpoint
    private Vector2 lastCheckpoint;
    private Vector2 lastChairPosition;

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
        if(lastCheckpoint == null)
        {
            Debug.LogError("Failed to set the last checkpoint upon spawning!");
        }
    }


    public void TakeDamage(int damage)
    {
        if(isInvincible)
        {
            Debug.Log("Player is Invincible!");
            return;
        }
        // Clamp the current health to be between 0 and max health
        if(ExpansionChipStatus.instance.isOverclocked)
        { 
            // If the ExpansionChipStatus is overclocked, the player will receive more damage
            currentHealth = Mathf.Clamp(currentHealth - (damage * 2), 0, maxHealth);
        } 
        else currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player is Dead");
            RespawnToChair();
        }
        else
        {
            GetComponent<Animator>().Play("Hit");
            
            // Knock the player backwards and upwards
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float pushDirection = GetComponent<PlatformerMovement2D>().IsLookingRight ? -1 : 1;
                rb.AddForce(new Vector2(pushDirection * knockbackForce, knockbackForce), ForceMode2D.Impulse);
            }
            else Debug.LogError("Failed to get the Rigidbody2D component!");

            Invincible(invincibilityTime);
        }
    }
    public void ReduceHealth(int healthToSacrifice)
    {
        currentHealth = Mathf.Clamp(currentHealth - healthToSacrifice, 0, maxHealth);
    }
    public void IncreaseHealth(int healthToAdd)
    {
        currentHealth = Mathf.Clamp(currentHealth + healthToAdd, 0, maxHealth);
        Debug.Log("Player's Health increased by " + healthToAdd);
    }


    public void Invincible(int IFrameTime)
    {
        StartCoroutine(IFrame(IFrameTime));
    }

    private IEnumerator IFrame(int IFrameTime)
    {
        isInvincible = true;
        Debug.Log("Player is Invincible for " + IFrameTime + " second");

        // Assuming you have a reference to the SpriteRenderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Flicker for a certain amount of time (IFrameTime)
        float endTime = Time.time + IFrameTime;
        while (Time.time < endTime)
        {
            // Toggle visibility
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // Wait for a short period of time
            yield return new WaitForSeconds(0.1f);
        }

        // Ensure the sprite is enabled at the end
        spriteRenderer.enabled = true;
        isInvincible = false;
        Debug.Log("Player is no longer Invincible! Be careful!");
    }

    // Check if the character hit a collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("FallingZone"))
        {
            isInvincible = false;
            ForceMoveToSafePosition();
            ReduceHealth(1);
        }

        if(collision.CompareTag("Checkpoint"))
        {
            lastCheckpoint = collision.transform.position;
            CameraManager.instance.SetTempCamera();
        }

        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
    private void RespawnToChair()
    {
        // Respawn the player at the last checkpoint
        transform.position = lastCheckpoint;
        currentHealth = maxHealth;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }
    private void ForceMoveToSafePosition()
    {
        transform.position = lastCheckpoint;
    }
    public void FullyHealHP()
    {
        currentHealth = maxHealth;
    }

    private void SaveChairPosition(Vector2 chairPosition)
    {
        lastChairPosition = chairPosition;
    }
}