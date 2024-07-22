using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private Animator transitionAnim;
    private GameObject playerUICanvas;
    private Animator playerUICanvasAnim;
    private string levelName;
    private Transform desiredPosition;
    // private Cinemachine.CinemachineVirtualCamera vcam1;
    // private Cinemachine.CinemachineVirtualCamera vcam2;
    private GameObject MainCamera;
    private GameObject SecondCamera;
    private void Start()
    {
        transitionAnim = GetComponent<Animator>();
        playerUICanvas = GameObject.FindWithTag("PlayerUI");
        playerUICanvasAnim = playerUICanvas.GetComponent<Animator>();
    }

    // Animation event!
    public void LoadLevel()
    {
        // Change Scene
        if(levelName != null && desiredPosition == null)
        {
            SceneManager.LoadScene(levelName);
            levelName = null;
            desiredPosition = null;
        }
        // Change Location
        else if(levelName == null && desiredPosition != null)
        {
            PlatformerMovement2D.instance.transform.position = desiredPosition.position;
            MainCamera.SetActive(false);
            SecondCamera.SetActive(true);
            
            levelName = null;
            desiredPosition = null;
        }
        else
        {
            Debug.LogError("2 of them are null or both are not null! This is not allowed.");
        }
        
    }
    public void TriggerLoading(string levelName)
    {
        if(levelName == null)
        {
            Debug.LogError("Level name is null");
            return;
        }
        else
        {
            this.levelName = levelName;
            transitionAnim.SetTrigger("Trigger");
            playerUICanvasAnim.SetTrigger("Trigger");
        }        
    }
    public void TriggerChangePosition(Transform desiredPosition, GameObject MainCamera, GameObject SecondCamera)
    {
        if(desiredPosition == null)
        {
            Debug.LogError("Desired position is null");
            return;
        }
        else
        {
            this.MainCamera = MainCamera;
            this.SecondCamera = SecondCamera;
            this.desiredPosition = desiredPosition;
            transitionAnim.SetTrigger("Trigger");
            playerUICanvasAnim.SetTrigger("Trigger");
        }
    }
    public void BlockInput()
    {
        PlatformerMovement2D.instance.blocked = true;
        CaptainMoonBlade.blocked = true;
    }
    public void UnblockInput()
    {
        PlatformerMovement2D.instance.blocked = false;
        CaptainMoonBlade.blocked = false;
    }
}
