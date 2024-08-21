using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RestOnChair : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    private DataPersistenceManager dataPersistenceManager;
    private bool nearTheChair = false;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        textMeshPro.gameObject.SetActive(false);
        
        dataPersistenceManager = DataPersistenceManager.instance;
        if(dataPersistenceManager == null)
        {
            Debug.LogError("DataPersistenceManager is not found in the scene.");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textMeshPro.gameObject.SetActive(true);
            nearTheChair = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textMeshPro.gameObject.SetActive(false);
            nearTheChair = false;
        }
    }

    private void Update()
    {
        if(playerInput.Player.Interact.WasReleasedThisFrame() && nearTheChair)
        {
            CameraManager.instance.SetChairCamera();
            dataPersistenceManager.SaveGame();
            Debug.Log("Game saved");
        }
    }
}
