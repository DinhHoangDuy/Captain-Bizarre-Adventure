using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Configuration")] [SerializeField]
    private string fileName;

    [SerializeField] private bool useEncryption = false;
    
    private FileDataHandler fileDataHandler;
    public static DataPersistenceManager instance { get; private set; }
    private List<IDataPersistence> dataPersistenceObjects;
    private GameData gameData;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one DataPersistentManager instance in the scene.");
        }
        instance = this;
    }

    private void Start()
    {
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load game data from file using a data handler
        this.gameData = fileDataHandler.Load();
        // if no data is found, create a new game
        if (this.gameData == null)
        {
            Debug.Log("No game data found. Initiating data to defaults");
            NewGame();
        }
        
        // Push the loaded data to other scripts that need it
        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        // Pass the game data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }
        
        // Save the game data to a file using a data handler
        fileDataHandler.Save(gameData);
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.FullyHealHP();
            Debug.Log("Player health fully healed after saving the game!");
        }
        else Debug.LogError("PlayerHealth script not found in the scene."); 
    }

    // private void OnApplicationQuit()
    // {
    //     SaveGame();
    // }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistencesObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistencesObjects);
    }
}