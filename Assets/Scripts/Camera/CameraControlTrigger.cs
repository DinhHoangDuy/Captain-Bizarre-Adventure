using UnityEngine;
using UnityEditor;
using Unity.Cinemachine;

public class CameraControlTrigger : MonoBehaviour
{
    public CameraControlTriggerCustomInspectionObjects customInspectionObjects = new CameraControlTriggerCustomInspectionObjects(); // Initialize here 
    private Collider2D triggerCollider;
    void Start()
    {
        triggerCollider = GetComponent<Collider2D>();    
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {          
            if (customInspectionObjects.panCameraOnContact)
            {
                // Pan the camera
                CameraManager.instance.PanCameraOnContact(customInspectionObjects.panDistance, customInspectionObjects.panTime, customInspectionObjects.panDirection, false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D player)
    {
        //======= Swap the camera when the player exits the trigger =======
        Vector2 exitDirection = (player.transform.position - triggerCollider.bounds.center).normalized;
        if(customInspectionObjects.swapCameraHorizontally && customInspectionObjects.leftVirtualCamera != null && customInspectionObjects.rightVirtualCamera != null)
        {
            // Swap the camera
            CameraManager.instance.SwapCameraLeftRight(customInspectionObjects.leftVirtualCamera, customInspectionObjects.rightVirtualCamera, exitDirection);
        }
        if(customInspectionObjects.swapCameraVertically && customInspectionObjects.topVirtualCamera != null && customInspectionObjects.bottomVirtualCamera != null)
        {
            // Swap the camera
            CameraManager.instance.SwapCameraTopBottom(customInspectionObjects.topVirtualCamera, customInspectionObjects.bottomVirtualCamera, exitDirection);
        }

        if (player.CompareTag("Player"))
        {
            if (customInspectionObjects.panCameraOnContact)
            {
                // Pan the camera
                CameraManager.instance.PanCameraOnContact(customInspectionObjects.panDistance, customInspectionObjects.panTime, customInspectionObjects.panDirection, true);
            }
        }
    }
}

[System.Serializable]
public class CameraControlTriggerCustomInspectionObjects
{
    // Will appear in the Editor
    public bool swapCameraHorizontally = false;
    [HideInInspector] public CinemachineCamera leftVirtualCamera;
    [HideInInspector] public CinemachineCamera rightVirtualCamera;

    public bool swapCameraVertically = false;
    [HideInInspector] public CinemachineCamera topVirtualCamera;
    [HideInInspector] public CinemachineCamera bottomVirtualCamera;

    public bool panCameraOnContact = false; 
    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum PanDirection
{
    Left,
    Right,
    Up,
    Down
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    void OnEnable()
    {
        // Initialize cameraControlTrigger with the target object
        cameraControlTrigger = (CameraControlTrigger)target;
        // Ensure customInspectionObjects is not null
        if (cameraControlTrigger.customInspectionObjects == null)
        {
            cameraControlTrigger.customInspectionObjects = new CameraControlTriggerCustomInspectionObjects();
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
    CameraControlTrigger cameraControlTrigger;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        // Ensure customInspectionObjects is not null
        if (cameraControlTrigger.customInspectionObjects != null)
        {
            // Sway Camera custom inspection
            if (cameraControlTrigger.customInspectionObjects.swapCameraHorizontally)
            {
                // Ensure the objects and properties you access are initialized
                // Example for leftVirtualCamera and rightVirtualCamera
                cameraControlTrigger.customInspectionObjects.leftVirtualCamera = EditorGUILayout.ObjectField("Left Virtual Camera", cameraControlTrigger.customInspectionObjects.leftVirtualCamera, typeof(CinemachineCamera), true) as CinemachineCamera;
                cameraControlTrigger.customInspectionObjects.rightVirtualCamera = EditorGUILayout.ObjectField("Right Virtual Camera", cameraControlTrigger.customInspectionObjects.rightVirtualCamera, typeof(CinemachineCamera), true) as CinemachineCamera;
            }
            if(cameraControlTrigger.customInspectionObjects.swapCameraVertically)
            {
                // Similar null checks and initialization should be ensured for topVirtualCamera and bottomVirtualCamera
                cameraControlTrigger.customInspectionObjects.topVirtualCamera = EditorGUILayout.ObjectField("Top Virtual Camera", cameraControlTrigger.customInspectionObjects.topVirtualCamera, typeof(CinemachineCamera), true) as CinemachineCamera;
                cameraControlTrigger.customInspectionObjects.bottomVirtualCamera = EditorGUILayout.ObjectField("Bottom Virtual Camera", cameraControlTrigger.customInspectionObjects.bottomVirtualCamera, typeof(CinemachineCamera), true) as CinemachineCamera;
            }

            // Pan Camera custom inspection

            if (cameraControlTrigger.customInspectionObjects.panCameraOnContact)
            {
                // Similar null checks and initialization should be ensured for panDirection, panDistance, and panTime
                cameraControlTrigger.customInspectionObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Pan Direction", cameraControlTrigger.customInspectionObjects.panDirection);
                cameraControlTrigger.customInspectionObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectionObjects.panDistance);
                cameraControlTrigger.customInspectionObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectionObjects.panTime);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("customInspectionObjects is not initialized.", MessageType.Warning);
        }

        if(GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}
#endif