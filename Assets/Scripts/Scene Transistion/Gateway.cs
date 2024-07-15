using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Transform desiredPosition;
    [SerializeField] private GameObject text;
    // [SerializeField] private Cinemachine.CinemachineVirtualCamera vcam1;
    // [SerializeField] private Cinemachine.CinemachineVirtualCamera vcam2;
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject SecondCamera;
    private LevelLoader levelLoader;
    // private BoxCollider2D boxCollider2D;
    private PlayerInput inputActions;
    private bool sceneNullReported = false;
    private bool positionNullReported = false;
    private bool inPosition = false; 

    private void Awake()
    {
        // boxCollider2D = GetComponent<BoxCollider2D>();
        levelLoader = FindObjectOfType<LevelLoader>();
        inputActions = new PlayerInput();
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    private void Update()
    {
        if (sceneName == "" && !sceneNullReported)
        {
            Debug.LogWarning("Scene name is null or empty");
            sceneNullReported = true;
            return;
        }
        else if(desiredPosition == null && !positionNullReported)
        {
            Debug.LogWarning("Desired position is null");
            positionNullReported = true;
            return;
        }

        if (desiredPosition != null && inPosition)
        {
            if(inputActions.Game.Interact.triggered)
            {   
                levelLoader.TriggerChangePosition(desiredPosition, MainCamera, SecondCamera);
            }      
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Automatically load the scene
            if(sceneName != "" && desiredPosition == null)
            {
                levelLoader.TriggerLoading(sceneName);
            }
            // Press the button to change position
            else if(sceneName == "" && desiredPosition != null)
            {
                text.SetActive(true);
                inPosition = true;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            text.SetActive(false);
            inPosition = false;
        }
    }
}
