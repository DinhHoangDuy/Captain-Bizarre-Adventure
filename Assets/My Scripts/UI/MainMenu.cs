using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
// TODO: Add a fade in animation for the main menu!!
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button firstSelectedMenuButton;

    [Header("Save Slot Menu")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;
    [SerializeField] private GameObject saveSlotMenuPanel;
    [SerializeField] private Button backButton;

    // private Animator mainMenuAnimator;
    private bool quitGame = false;

    private void Awake()
    {
        //Main Menu Button
        startGameButton.onClick.AddListener(StartGame);
        exitGameButton.onClick.AddListener(ExitGame);
        //Save Slot Menu Button
        backButton.onClick.AddListener(BackToMainMenu);
        // Main Menu Animator
        // mainMenuAnimator = GetComponent<Animator>();
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
    private void ExitGame()
    {
        DisableMainMenuButtons();
        DataPersistenceManager.instance.SaveGame();
        quitGame = true;
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
        // mainMenuAnimator.SetTrigger("Start"); // Trigger the fade out animation, then quit the game

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
