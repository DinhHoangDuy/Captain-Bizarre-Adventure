using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Current state and location
    public int currentMaxHealth;
    public float currentSP;
    public Vector3 lastSavedLocation;
    public string lastLocationSaved;
    public CinemachineCamera lastSavedChairVirtualCamera;
    public CinemachinePositionComposer lastSavedChairPositionComposer;

    // The time the game was saved
    public long saveTime;
    
    // Character Skill Set unlocked
    public bool doubleJumpUnlocked;
    public bool wallJumpUnlocked;
    public bool dashUnlocked;
    public bool ultimateUnlocked;
    public SerializableDictionary<string, bool> unlockedSkills;

    
    // Constructor to initialize the game data with default values
    public GameData()
    {
        // Default state and location
        currentMaxHealth = 5;
        currentSP = 20f;
        lastSavedLocation = new Vector3(-30, 20f, 0);
        
        lastLocationSaved = "Map 1";
        lastSavedChairVirtualCamera = null;
        lastSavedChairPositionComposer = null;
        
        // Default character skill set
        doubleJumpUnlocked  = false;
        dashUnlocked        = false;
        wallJumpUnlocked    = false;
        ultimateUnlocked    = false;
        unlockedSkills      = new SerializableDictionary<string, bool>();
    }
}