using UnityEngine;

public class DestroyableOjectsHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    private TakeDMG TakeDMGScript;

    private void Awake()
    {
        TakeDMGScript = GetComponent<TakeDMG>();
        if (TakeDMGScript != null)
        {
            TakeDMGScript.OnHitDestroyableReceived += ReduceHealth;
        }
    }
    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void ReduceHealth(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
