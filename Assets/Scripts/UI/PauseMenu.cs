using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public static bool isPaused;
    public GameObject pauseMenuCanvas;
    //public UnityEngine.UI.Button pauseButton;
    // Start is called before the first frame update

    void Start()
    {
        //=====Pause Menu=====
        //Preventing the game automatically paused when started
        Time.timeScale = 1f;
        pauseMenuCanvas.SetActive(false);        
        //pauseButton.onClick.AddListener(PauseBtnClick);

    }

    // Update is called once per frame
    void Update()
    {
        //Stop this script if the player is not supposed to be able to move or pause the game
        if(PlatformerMovement2D.blocked)
        {
            return;
        }

        //=====Pause Menu Trigger=====
        //bool pausePressed = InputManager.GetInstance().GetPausePressed();
        bool pausePressed = Input.GetButtonDown("Cancel");
        if (pausePressed)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    //=====Pause Menu functions=====
    /*
    void PauseBtnClick() //For Pause Menu Button
    {
        if (!isPaused)
        {
            PauseGame();
        }
    }
    */
    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
