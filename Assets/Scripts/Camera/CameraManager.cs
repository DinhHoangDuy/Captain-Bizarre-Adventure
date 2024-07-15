using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
// using System.Numerics;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineCamera[] _allVirtualCameras;
    private float targetYDamping;
    [Tooltip("Adjust this value as needed for smoother or faster transitions")] 
    [SerializeField] private float lerpSpeed = 2f;


    [Header("Camera Settings")]
    public float _fallSpeedYDampingChangeThreshold = -15f;

    private CinemachineCamera _currentVirtualCamera;
    private CinemachinePositionComposer _PositionComposer;

    private float _normYPanAmount;

    private Vector3 _startingTrackedObjectOffset;
    private Coroutine _panCameraCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if(_allVirtualCameras[i].enabled)
            {
                // Set the current virtual camera
                _currentVirtualCamera = _allVirtualCameras[i];

                // Set the current framing transposer
                _PositionComposer = _currentVirtualCamera.GetComponent<CinemachinePositionComposer>();
            }
        }
        // Set the YDamping amound so it's base from the inspector value
        _normYPanAmount = _PositionComposer.Damping.y;

        // Set the starting tracked object offset
        _startingTrackedObjectOffset = _PositionComposer.TargetOffset;
    }


    private void Update()
    {
        if (_PositionComposer.Damping.y != targetYDamping)
        {
            _PositionComposer.Damping.y = Mathf.Lerp(_PositionComposer.Damping.y, targetYDamping, lerpSpeed * Time.deltaTime);
        }
    }

    #region Y Damping
    public void LowYDamping()
    {
        targetYDamping = 0.1f;
    } 

    public void NormalYDamping()
    {
        targetYDamping = _normYPanAmount;
    }
    #endregion

    #region Pan Camera
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }
    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startPos = Vector2.zero;

        // handle pan from trigger
        if(!panToStartingPos)
        {
            // set direction and distance
            switch (panDirection)
            {
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                default: break;
            }
            endPos *= panDistance;
            startPos = _PositionComposer.TargetOffset;
            endPos += startPos;
        }
        // handle pan back to starting position
        else
        {
            startPos = _PositionComposer.TargetOffset;
            endPos = _startingTrackedObjectOffset;
        }

        // handle the actual panning of the camera
        float elapsedTime = 0;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 panLerp = Vector3.Lerp(startPos, endPos, elapsedTime / panTime);
            _PositionComposer.TargetOffset = panLerp;
            yield return null;
        }
    }
    #endregion

    #region Camera Swap
    public void SwapCameraLeftRight(CinemachineCamera leftCamera, CinemachineCamera rightCamera, Vector2 triggerExitDirection)
    {
        if (_currentVirtualCamera == leftCamera && triggerExitDirection.x > 0f)
        {
            // -------- Change the camera when the player exits the trigger from the left to the right-------
            // activate the right camera
            rightCamera.enabled = true;
            // deactivate the left camera
            leftCamera.enabled = false;
            // set the current camera to the right camera
            _currentVirtualCamera = rightCamera;
            // set the current framing transposer
            _PositionComposer = _currentVirtualCamera.GetComponent<CinemachinePositionComposer>();
        }
        else if (_currentVirtualCamera == rightCamera && triggerExitDirection.x < 0f)
        {
            // -------- Change the camera when the player exits the trigger from the right to the left-------
            // activate the left camera
            leftCamera.enabled = true;
            // deactivate the right camera
            rightCamera.enabled = false;
            // set the current camera to the right camera
            _currentVirtualCamera = leftCamera;
            // set the current framing transposer
            _PositionComposer = _currentVirtualCamera.GetComponent<CinemachinePositionComposer>();
        }
        
    }

    public void SwapCameraTopBottom(CinemachineCamera topCamera, CinemachineCamera bottomCamera, Vector2 triggerExitDirection)
    {
        if (_currentVirtualCamera == topCamera && triggerExitDirection.y < 0f)
        {
            //-------- Change the camera when the player exits the trigger from the top to the bottom-------
            // activate the bottom camera
            bottomCamera.enabled = true;
            // deactivate the top camera
            topCamera.enabled = false;
            // set the current camera to the bottom camera
            _currentVirtualCamera = bottomCamera;
            // set the current framing transposer
            _PositionComposer = _currentVirtualCamera.GetComponent<CinemachinePositionComposer>();
        }
        else if (_currentVirtualCamera == bottomCamera && triggerExitDirection.y > 0f)
        {
            // -------- Change the camera when the player exits the trigger from the bottom to the top-------
            // activate the top camera
            topCamera.enabled = true;
            // deactivate the bottom camera
            bottomCamera.enabled = false;
            // set the current camera to the top camera
            _currentVirtualCamera = topCamera;
            // set the current framing transposer
            _PositionComposer = _currentVirtualCamera.GetComponent<CinemachinePositionComposer>();
        }
        
    }
    #endregion
}