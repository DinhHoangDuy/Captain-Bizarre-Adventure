using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeNewGameIfNull = false;
    [SerializeField] private bool overrideProfileID = false;
    [SerializeField] private string testProfileID = "test";


    [Header("File Storage Configuration")] [SerializeField]
    private string fileName;

    [SerializeField] private bool useEncryption = false;
    public string selectedProfileID = "";
    
    private FileDataHandler fileDataHandler;
    public static DataPersistenceManager instance { get; private set; }
    private List<IDataPersistence> dataPersistenceObjects;
    private GameData gameData;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data persistence is disabled. No data will be saved or loaded.");
        }

        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        if (overrideProfileID)
        {
            this.selectedProfileID = testProfileID;
            Debug.LogWarning("Profile ID is overridden to " + testProfileID);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }   
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        SaveGame();
    }

    public void LoadGame()
    {
        if(disableDataPersistence)
        {
            Debug.LogWarning("Data persistence is disabled. No data will be saved or loaded.");
            return;
        }
        // Load game data from file using a data handler
        this.gameData = fileDataHandler.Load(selectedProfileID);

        if(this.gameData == null && initializeNewGameIfNull)
        {
            Debug.Log("No game data found. Creating a new game data based on the initialization settings.");
            NewGame();
        }

        // if no data is found, stop the loading process
        if (this.gameData == null)
        {
            Debug.Log("No game data found. A new game data needs to be created.");
            return;
        }
        
        // Push the loaded data to other scripts that need it
        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if(disableDataPersistence)
        {
            Debug.LogWarning("Data persistence is disabled. No data will be saved or loaded.");
            return;
        }

        // if no data is found, stop the saving process
        if (this.gameData == null)
        {
            Debug.LogWarning("No game data found. A new game data needs to be created.");
            return;
        }
        
        // Pass the game data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }

        // Set the timestamp for the save
        gameData.saveTime = System.DateTime.Now.ToBinary();
        
        // Save the game data to a file using a data handler
        fileDataHandler.Save(gameData, selectedProfileID);
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.FullyHealHP();
            Debug.Log("Player health fully healed after saving the game!");
        }
        else Debug.LogWarning("PlayerHealth script not found in the scene."); 
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistencesObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistencesObjects);
    }

    public bool HasGameData()
    {
        return this.gameData != null;
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileID = newProfileId;
        LoadGame();
    }

    public Dictionary<string, GameData> GetAllProfilesGameData() 
    {
        return fileDataHandler.LoadAllProfiles();
    }

    public string GetMostRecentlyUpdatedProfileID()
    {
        return fileDataHandler.GetMostRecentlyUpdatedProfileID();
    }
}