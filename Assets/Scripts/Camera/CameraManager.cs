using System.Runtime.CompilerServices;
using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEditor.EditorTools;
// using System.Numerics;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;
    private float targetYDamping;
    [Tooltip("Adjust this value as needed for smoother or faster transitions")] 
    [SerializeField] private float lerpSpeed = 2f;


    [Header("Camera Settings")]
    public float _fallSpeedYDampingChangeThreshold = -15f;

    private CinemachineVirtualCamera _currentVirtualCamera;
    private CinemachineFramingTransposer _framingTransposer;

    private float _normYPanAmount;

    private Vector2 _startingTrackedObjectOffset;
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
                _framingTransposer = _currentVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }
        // Set the YDamping amound so it's base from the inspector value
        _normYPanAmount = _framingTransposer.m_YDamping;

        // Set the starting tracked object offset
        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }


    private void Update()
    {
        if (_framingTransposer.m_YDamping != targetYDamping)
        {
            _framingTransposer.m_YDamping = Mathf.Lerp(_framingTransposer.m_YDamping, targetYDamping, lerpSpeed * Time.deltaTime);
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
            startPos = _framingTransposer.m_TrackedObjectOffset;
            endPos += startPos;
        }
        // handle pan back to starting position
        else
        {
            startPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        // handle the actual panning of the camera
        float elapsedTime = 0;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 panLerp = Vector3.Lerp(startPos, endPos, elapsedTime / panTime);
            _framingTransposer.m_TrackedObjectOffset = panLerp;
            yield return null;
        }
    }
    #endregion

    #region Camera Swap
    public void SwapCamera(CinemachineVirtualCamera leftCamera, CinemachineVirtualCamera rightCamera, Vector2 triggerExitDirection)
    {
        // if the current camera is the left camera and the trigger exit direction is on the right
        if (_currentVirtualCamera == leftCamera && triggerExitDirection.x > 0f)
        {
            // activate the right camera
            rightCamera.enabled = true;
            // deactivate the left camera
            leftCamera.enabled = false;
            // set the current camera to the right camera
            _currentVirtualCamera = rightCamera;
            // set the current framing transposer
            _framingTransposer = _currentVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        // if the current camera is the left camera and the trigger exit direction is on the right
        else if (_currentVirtualCamera == rightCamera && triggerExitDirection.x < 0f)
        {
            // activate the left camera
            leftCamera.enabled = true;
            // deactivate the right camera
            rightCamera.enabled = false;
            // set the current camera to the right camera
            _currentVirtualCamera = leftCamera;
            // set the current framing transposer
            _framingTransposer = _currentVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        
    }
    #endregion
}