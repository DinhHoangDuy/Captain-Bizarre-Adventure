using System;
using UnityEngine;
using UnityEngine.UI;
// using UnityEditor;
// using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button firstSelectedMenuButton;
    [SerializeField] private bool isUsingAnimation;
    public NextAction nextAction;
    private String mapSceneName;

    [Header("Save Slot Menu")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;
    [SerializeField] private GameObject saveSlotMenuPanel;
    [SerializeField] private Button backButton;

    private Animator mainMenuAnimator;
    public static MainMenu instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //Main Menu Button
        startGameButton.onClick.AddListener(StartGame);
        exitGameButton.onClick.AddListener(ExitGameAnimation);
        //Save Slot Menu Button
        backButton.onClick.AddListener(BackToMainMenu);
        // Main Menu Animator
        mainMenuAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        // Automatically enable the main menu panel, then select the first button when the game starts
        mainMenuPanel.SetActive(true);
        firstSelectedMenuButton.Select();
        // Automatically disable the save slot menu when the game starts
        saveSlotMenuPanel.SetActive(false);
    }

    private void StartGame()
    {
        // Open the save slot menu
        OpenSaveSlotMenu();
    }
    private void ExitGameAnimation()
    {
        DisableMainMenuButtons();
        nextAction = NextAction.ExitGame;
        if (isUsingAnimation)
        {
            mainMenuAnimator.SetTrigger("Start");
        }
        else
        {
            ExitGame();
        }
    }
    public void LoadGameAnimation(String mapSceneName)
    {
        nextAction = NextAction.StartGame;
        this.mapSceneName = mapSceneName;
        if (isUsingAnimation)
        {
            mainMenuAnimator.SetTrigger("Start");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(mapSceneName);
        }
    }

    // Animation events
    public void ExitGame()
    {
        if (nextAction == NextAction.ExitGame)
        {
            DataPersistenceManager.instance.SaveGame();
            Application.Quit();
        }
    }
    public void LoadSaveGame()
    {
        if (nextAction == NextAction.StartGame)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(mapSceneName);
        }
    }
    internal void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        saveSlotMenuPanel.SetActive(false);
        EnableMainMenuButtons();
        firstSelectedMenuButton.Select();

    }
    private void OpenSaveSlotMenu()
    {
        DisableMainMenuButtons();
        mainMenuPanel.SetActive(false);
        saveSlotMenuPanel.SetActive(true);
        saveSlotsMenu.ActivateMenu();
    }

    private void DisableMainMenuButtons()
    {
        startGameButton.interactable = false;
        exitGameButton.interactable = false;
    }
    private void EnableMainMenuButtons()
    {
        startGameButton.interactable = true;
        exitGameButton.interactable = true;

    }
}

public enum NextAction
{
    StartGame,
    ExitGame
}
