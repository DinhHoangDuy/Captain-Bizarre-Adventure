using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 300f;
    float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeMeleeDamage(float damage, string Type)
    {
        currentHealth -= damage;
        // TODO: Play Hurt Animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void TakeRangedDamage(float damage, string Type)
    {
        currentHealth -= damage;
        // TODO: Play Hurt Animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void TakeDOTEffect(float DOTDamage, int Hits, string Type)
    {
        while(Hits > 0)
        {
            TakeDOTDamage(DOTDamage, Type);
            Hits--;
        }
    }
    private void TakeDOTDamage(float damage, string Type)
    {
        currentHealth -= damage;
        // TODO: Play Hurt Animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy Died!");
        // TODO: Play Die Animation
        GetComponent<Collider2D>().enabled = false; //Disable Collider of the enemy
        this.enabled = false;
    }
}