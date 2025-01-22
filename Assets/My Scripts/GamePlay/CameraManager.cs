using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField] private CinemachineCamera defaultVirtualCamera; // This is used only one when the game starts for the first time
    private CinemachineCamera[] cinemachineCameras;
    private CinemachineCamera lastSavedChairVirtualCamera; // This is used to store the last chair Cinemachine Camera, so that we can switch back to it when the player gets up from the chair when needed
    private CinemachineCamera lastSavedCheckpointVirtualCamera; // This is used to store the last checkpoint Cinemachine Camera, so that we can switch back to it when the player respawns at the checkpoint when needed
    private CinemachineCamera currentActiveVirtualCamera; // This is used to store the current active Cinemachine Camera

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        cinemachineCameras = FindObjectsOfType<CinemachineCamera>();
        if (cinemachineCameras.Length == 0)
        {
            Debug.LogError("No Cinemachine Camera found in the scene.");
        }

        // Check if all Cinemachine Cameras have the CameraAutoEnable script attached
        foreach (CinemachineCamera camera in cinemachineCameras)
        {
            if (!camera.GetComponent<CameraAutoEnable>())
            {
                Debug.LogError("Cinemachine Camera " + camera.name + " does not have the CameraAutoEnable script attached.");
            }
        }
    }

    void Update()
    {
        // Check if more than one Cinemachine Camera is active
        int activeCameras = 0;
        foreach (CinemachineCamera camera in cinemachineCameras)
        {
            if (camera.enabled)
            {
                activeCameras++;
            }
        }
        if (activeCameras > 1)
        {
            Debug.LogError("More than one Cinemachine Camera is active! This is not allowed.");
        }

        // Check if no Cinemachine Camera is active. If true, enable the default Cinemachine Camera
        if (activeCameras == 0)
        {
            defaultVirtualCamera.enabled = true;
        }

        // Get the current active Cinemachine Camera
        currentActiveVirtualCamera = GetCurrentActiveCamera();
    }

    #region Current Cameras
    internal CinemachineCamera GetCurrentActiveCamera()
    {
        foreach (CinemachineCamera camera in cinemachineCameras)
        {
            if (camera.enabled)
            {
                return camera;
            }
        }
        return null;
    }
    internal void SetLastSavedChairVirtualCamera()
    {
        lastSavedChairVirtualCamera = GetCurrentActiveCamera();
    }
    internal void LoadLastChairVirtualCamera()
    {
        UseThisCamera(lastSavedChairVirtualCamera);
    }
    internal void SetLastSavedCheckpointVirtualCamera(CinemachineCamera checkpointVirtualCamera)
    {
        lastSavedCheckpointVirtualCamera = checkpointVirtualCamera;
    }
    #endregion

    #region Enable/Disable Cinemachine Camera
    internal void DisableAllCameras()
    {
        foreach (CinemachineCamera camera in cinemachineCameras)
        {
            camera.enabled = false;
        }
    }
    #endregion

    #region Change Camera
    internal void SwitchCamera(CinemachineCamera cameraA, CinemachineCamera cameraB)
    {
        cameraA.enabled = !cameraA.enabled;
        cameraB.enabled = !cameraB.enabled;
    }
    internal void UseThisCamera(CinemachineCamera camera)
    {
        DisableAllCameras();
        camera.enabled = true;
    }
    #endregion
}
