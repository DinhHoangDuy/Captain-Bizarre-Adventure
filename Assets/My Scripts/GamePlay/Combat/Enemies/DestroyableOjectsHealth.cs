using UnityEngine;

public class DestroyableOjectsHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ReduceHealth(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
