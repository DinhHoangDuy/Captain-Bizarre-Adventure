
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GateWay : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Transform desiredPosition;
    [SerializeField] private GameObject textGameObject;
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject SecondCamera;
    [SerializeField] private bool interactionRequired = false;
    private LevelLoader levelLoader;
    private PlayerInput inputActions;
    private bool sceneNullReported = false;
    private bool positionNullReported = false;
    private bool inPosition = false; 

    private void Awake()
    {
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

        if (inPosition && interactionRequired && inputActions.Game.Interact.triggered)
        {
            if(desiredPosition != null)
            {   
                levelLoader.TriggerChangePosition(desiredPosition, MainCamera, SecondCamera);
            } 
            else if(sceneName != "")
            {
                levelLoader.TriggerLoading(sceneName);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Load the scene
            if(sceneName != "" && desiredPosition == null)
            {
                if (!interactionRequired)
                {
                    levelLoader.TriggerLoading(sceneName);
                }
                else
                {
                    textGameObject.SetActive(true);
                    inPosition = true;
                }
            }
            // Change position
            else if(sceneName == "" && desiredPosition != null)
            {
                if (!interactionRequired)
                {
                    levelLoader.TriggerChangePosition(desiredPosition, MainCamera, SecondCamera);
                }
                else
                {
                    textGameObject.SetActive(true);
                    inPosition = true;
                }
            }
            else
            {
                Debug.LogError("2 of them are null or both are not null! This is not allowed.");
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textGameObject.SetActive(false);
            inPosition = false;
        }
    }
}
