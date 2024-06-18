using UnityEngine;
using UnityEditor;
using Cinemachine;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectionObjects customInspectionObjects = new CustomInspectionObjects(); // Initialize here 
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
        Vector2 exitDirection = (player.transform.position - triggerCollider.bounds.center).normalized;
        if(customInspectionObjects.swapCamera && customInspectionObjects.leftVirtualCamera != null && customInspectionObjects.rightVirtualCamera != null)
        {
            // Swap the camera
            CameraManager.instance.SwapCamera(customInspectionObjects.leftVirtualCamera, customInspectionObjects.rightVirtualCamera, exitDirection);
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
public class CustomInspectionObjects
{
    // Will appear in the Editor
    public bool swapCamera = false;
    public bool panCameraOnContact = false;
    [HideInInspector] public CinemachineVirtualCamera leftVirtualCamera;
    [HideInInspector] public CinemachineVirtualCamera rightVirtualCamera;

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
            cameraControlTrigger.customInspectionObjects = new CustomInspectionObjects();
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
            if (cameraControlTrigger.customInspectionObjects.swapCamera)
            {
                // Ensure the objects and properties you access are initialized
                // Example for leftVirtualCamera and rightVirtualCamera
                cameraControlTrigger.customInspectionObjects.leftVirtualCamera = EditorGUILayout.ObjectField("Left Virtual Camera", cameraControlTrigger.customInspectionObjects.leftVirtualCamera, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
                cameraControlTrigger.customInspectionObjects.rightVirtualCamera = EditorGUILayout.ObjectField("Right Virtual Camera", cameraControlTrigger.customInspectionObjects.rightVirtualCamera, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            }

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