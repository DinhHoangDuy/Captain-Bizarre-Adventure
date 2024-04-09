using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    private bool isTriggered = false;
    public bool TriggerStatus() => isTriggered;

    // Trigger properties
    [SerializeField] private bool pernamantTrigger = false;
    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("Player triggered the trap!");
        }
    }
    private void OnTriggerExit2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            if (!pernamantTrigger)
            {
                isTriggered = false;
                Debug.Log("Player left the trap!");
            }
        }
    }

    private void Update()
    {
        // Always check if the trap is not triggered if the trap is not a pernamant trigger, and there's not a player in the trigger area
        if (!pernamantTrigger && !isTriggered)
        {
            isTriggered = false;
        }
    }
}