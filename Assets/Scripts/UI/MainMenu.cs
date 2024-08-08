using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
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

    private void Awake()
    {
        //Main Menu Button
        startGameButton.onClick.AddListener(StartGame);
        exitGameButton.onClick.AddListener(ExitGame);
        //Save Slot Menu Button
        backButton.onClick.AddListener(BackToMainMenu);
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
        // if(!DataPersistenceManager.instance.HasGameData())
        // {
        //     Debug.Log("No game data found. Creating a new game data.");
        //     DataPersistenceManager.instance.NewGame();
        // }

        // //Load the game scene
        // SceneManager.LoadSceneAsync("Scenes/Stages/Maps/Map 1");
        
        // // Open the save slot menu
        OpenSaveSlotMenu();
    }
    private void ExitGame()
    {
        DisableMainMenuButtons();
        DataPersistenceManager.instance.SaveGame();
        //Quit the game if the game is running in the build, or stop the game view if the game is running in the editor
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
    private void BackToMainMenu()
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
