using System;
using TMPro;
using UnityEngine;

public class InteractionBackend : MonoBehaviour
{
    [SerializeField] private TextMeshPro interactionText;
    private bool isInTheZone = false;
    private PlayerInput _inputAction;
    
    // Vending Machine
    private VendingMachine vendingMachine;
    
    void Awake()
    {
        _inputAction = new PlayerInput();
        vendingMachine = GetComponent<VendingMachine>();
    }

    private void OnEnable()
    {
        _inputAction.Enable();
    }

    private void OnDisable()
    {
        _inputAction.Disable();
    }

    void Start()
    {
        interactionText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(_inputAction.Player.Interact.triggered && isInTheZone)
        {
            Debug.Log("Interaction Triggered by player");
            if (vendingMachine != null)
            {
                vendingMachine.UseVendingMachine();
            }
            else
            {
                Debug.LogWarning("Vending Machine is not attached to the object. This might not be a vending machine");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            interactionText.gameObject.SetActive(true);
            isInTheZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            interactionText.gameObject.SetActive(false);
            isInTheZone = false;
        }
    } 
}
