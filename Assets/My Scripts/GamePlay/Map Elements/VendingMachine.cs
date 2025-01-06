using UnityEngine;


// The frontend for interaction with the player.
[RequireComponent(typeof(InteractionBackend))]
public class VendingMachine : MonoBehaviour
{
    [SerializeField] private VendingMachineType vendingMachineType;
    [SerializeField] private int vendingMachineValue;

    private InteractionBackend interactionBackend;
    void Start()
    {
        interactionBackend = GetComponent<InteractionBackend>();
    }

    void Update()
    {
        if (interactionBackend.interactionTriggered)
        {
            UseVendingMachine();
        }
    }

    public void UseVendingMachine()
    {
        interactionBackend.interactionTriggered = false;
        if (vendingMachineType == VendingMachineType.Health)
        {
            Debug.Log("Health Vending Machine used");
            var playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogError("PlayerHealth component not found on CaptainMoonBlade");
            }
            else
            {
                if (playerHealth.currentHealth == playerHealth.maxHealth)
                {
                    Debug.Log("Health is already full");
                }
                Debug.Log($"Increasing health by {vendingMachineValue} health");
                playerHealth.IncreaseHealth(vendingMachineValue);
            }
        }
        else if (vendingMachineType == VendingMachineType.SP)
        {
            Debug.Log("Energy Vending Machine used");
            var playerSP = GameObject.FindWithTag("Player").GetComponent<PlayerSP>();
            if (playerSP == null)
            {
                Debug.LogError("The component is not found");
            }
            else
            {
                if (playerSP._currentSP == playerSP._maxSP)
                {
                    Debug.Log("SP is already full");
                }
                Debug.Log($"Increasing SP by {vendingMachineValue}%");
                playerSP.IncreaseSPByPersent(vendingMachineValue);
            }
        }
    }
}
public enum VendingMachineType
{
    Health,
    SP,
}
