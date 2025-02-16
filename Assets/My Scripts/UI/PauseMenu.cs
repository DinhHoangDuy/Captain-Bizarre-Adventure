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
    [SerializeField] private Button firstSelectedMenuButton;
    [SerializeField] private string mainMenuScene = "My Scenes/MainMenu/Welcome";
    [Header("Player Guide")]
    [SerializeField] private GameObject playerGuidePanel;
    public static bool isPaused = false;
    public bool isUsingPlayerGuide = false;
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button gameExitButton;
    [SerializeField] private Button playerGuideButton;
    [SerializeField] private Button guideExitButton;
    //Adapt new Input System
    private PlayerInput playerInput;
    private InputAction pauseInput;
    private void Awake()
    {
        playerInput = new PlayerInput();
        //Pause Menu Buttons
        resumeButton.onClick.AddListener(ResumeGame);
        gameExitButton.onClick.AddListener(ExitGame);
        playerGuideButton.onClick.AddListener(OpenPlayerGuidePanel); 
        guideExitButton.onClick.AddListener(ClosePlayerGuidePanel);
    }

    private void OnEnable()
    {
        pauseInput = playerInput.Player.Pause;
        pauseInput.performed += PauseMenuPanel;
        pauseInput.Enable();
    }
    private void OnDisable()
    {
        pauseInput.Disable();
    }

    private void Start()
    {
        //Preventing the game automatically paused when started
        Time.timeScale = 1f;
        // Hide the Pause Menu Panel and Player Guide Panel when the game starts
        pauseMenuPanel.SetActive(false);  
        playerGuidePanel.SetActive(false);   
    }

    // Update is called once per frame
    private void Update()
    {
        //Auto enable pause menu Panel 
        pauseMenuPanel.SetActive(isPaused);

        if (isPaused || isUsingPlayerGuide)
        {
            Time.timeScale = 0f;
        }
        else if (!isPaused && !isUsingPlayerGuide)
        {
            Time.timeScale = 1f;
        }

        PlatformerMovement2D.instance.inputBlocked = isPaused;
    }

    #region Pause Mene Functions
    //pauseInput trigger method
    private void PauseMenuPanel(InputAction.CallbackContext context)
    {
        if (isPaused && !isUsingPlayerGuide)
        {
            ResumeGame();
        }
        else if (isUsingPlayerGuide)
        {
            ClosePlayerGuidePanel();
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
        firstSelectedMenuButton.Select();
        isPaused = true;     
    }
    public void ResumeGame()
    {
        isPaused = false;
    }
    public void ExitGame()
    {
        isPaused = false;
        SceneManager.LoadScene(mainMenuScene);
    }
    #endregion

    #region Player Guide Functions
    private void OpenPlayerGuidePanel()
    {
        // Hide the Pause Menu Panel and show the Player Guide Panel
        pauseMenuPanel.SetActive(false);
        playerGuidePanel.SetActive(true);
        isUsingPlayerGuide = true;
        guideExitButton.Select();
    }
    private void ClosePlayerGuidePanel()
    {
        // Hide the Player Guide Panel and show the Pause Menu Panel
        playerGuidePanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        isUsingPlayerGuide = false;
    }
    #endregion
}
