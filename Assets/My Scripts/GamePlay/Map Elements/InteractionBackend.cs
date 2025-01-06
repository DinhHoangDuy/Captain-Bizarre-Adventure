using System;
using TMPro;
using UnityEngine;

// The backend for interaction with the player
public class InteractionBackend : MonoBehaviour
{
    [SerializeField] private TextMeshPro interactionText;
    private bool isInTheZone = false;
    private PlayerInput _inputAction;
    internal bool interactionTriggered = false;
    
    void Awake()
    {
        _inputAction = new PlayerInput();
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
            interactionTriggered = true;
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
