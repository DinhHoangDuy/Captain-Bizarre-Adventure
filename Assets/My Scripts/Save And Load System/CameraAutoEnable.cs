using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraAutoEnable : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string cameraID;
    [ContextMenu("Set Camera ID")]
    private void SetCameraID()
    {
        cameraID = System.Guid.NewGuid().ToString();
    }

    private CinemachineCamera cinemachineCamera;
    private bool isActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        // if (isActive)
        // {
        //     cinemachineCamera.enabled = true;
        // }
        // else
        // {
        //     cinemachineCamera.enabled = false;
        // }
    }

    #region  Save and Load Data
    public void LoadData(GameData data)
    {
        data.cameras.TryGetValue(cameraID, out isActive);
        if (isActive)
        {
            cinemachineCamera.enabled = true;
        }
        else
        {
            cinemachineCamera.enabled = false;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.cameras.ContainsKey(cameraID))
        {
            data.cameras.Remove(cameraID);
        }
        data.cameras.Add(cameraID, cinemachineCamera.enabled);
    }
    #endregion
}
