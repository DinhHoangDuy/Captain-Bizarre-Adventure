using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private UnityEngine.UI.Button pauseButton;
    public static bool isPaused = false;
    //Adapt new Input System
    private PlatformerInputAction platformerInputaction;
    private InputAction pauseInput;
    private void Awake()
    {
        platformerInputaction = new PlatformerInputAction();
    }
    private void OnEnable()
    {
        pauseInput = platformerInputaction.Player.Pause;
        pauseInput.performed += PauseMenuPanel;
        pauseInput.Enable();
    }
    private void OnDisable()
    {
        pauseInput.Disable();
    }

    private void Start()
    {
        //=====Pause Menu=====
        //Preventing the game automatically paused when started
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);        
        pauseButton.onClick.AddListener(PauseBtnClick);
    }

    // Update is called once per frame
    private void Update()
    {
        //Auto enable pause menu Panel 
        pauseMenuPanel.SetActive(isPaused);
    }

    //pauseInput trigger method
    private void PauseMenuPanel(InputAction.CallbackContext context)
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
    //=====Pause Menu functions=====
    void PauseBtnClick() //For Pause Menu Button
    {
        if (!isPaused)
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;        
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
}
