using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    public static bool isPaused = false;
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    //Adapt new Input System
    private PlayerInput playerInput;
    private InputAction pauseInput;
    private void Awake()
    {
        playerInput = new PlayerInput();
        //Pause Menu Button
        resumeButton.onClick.AddListener(ResumeGame);
    }
    private void OnEnable()
    {
        pauseInput = playerInput.Game.Pause;
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
        // pauseMenuPanel.SetActive(false);     
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
        PlatformerMovement2D.blocked = true;       
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        PlatformerMovement2D.blocked = false;
    }
}
