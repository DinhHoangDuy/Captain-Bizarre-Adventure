/* 
- This script manages the health of an enemy in a game.
- It requires an EnemyResistance component on the same GameObject.
- This is only needed for enemies that can take damage, not for Dummies or destructible objects (which takes hit counts only. They will have a different script for that).
- The script has a TakeDamage method that reduces the enemy's health by the amount of damage taken.
- When the enemy's health reaches zero, the Die method is called, which plays the Death Animation and destroys the GameObject afterwards.
*/

using UnityEngine;

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
