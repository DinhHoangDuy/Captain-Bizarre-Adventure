using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TakeDMG))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    public int maxHealth { get { return _maxHealth; } }
    public int currentHealth { get; private set; }
    private bool isInvincible = false;

    private void Awake()
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
    }


    private void TakeDamage(int damage)
    {
        if(isInvincible)
        {
            Debug.Log("Player is Invincible!");
            return;
        }
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player is Dead");

            //Destroy the player when the health is 0
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(IFrame());
        }
    }
    public void SacrificiceHealth(int healthToSacrifice)
    {
        currentHealth = Mathf.Clamp(currentHealth - healthToSacrifice, 0, maxHealth);
    }

    private IEnumerator IFrame()
    {
        isInvincible = true;
        Debug.Log("Player is Invincible for 1 second");

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

        isInvincible = false;
        Debug.Log("Player is no longer Invincible! Be careful!");
    }
}