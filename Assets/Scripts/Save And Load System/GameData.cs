using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Current state and location
    public int currentMaxHealth;
    public float currentSP;
    public Vector3 lastSavedLocation;
    public string lastSavedScene;
    
    // Character Skill Set unlocked
    public bool doubleJumpAllowed;
    public bool wallJumpAllowed;
    public bool dashAllowed;
    public SerializableDictionary<string, bool> unlockedSkills;

    
    // Constructor to initialize the game data with default values
    public GameData()
    {
        // Default state and location
        currentMaxHealth = 5;
        currentSP = 20f;
        lastSavedLocation = new Vector3(0, 0, 0);
        // Default character skill set
        doubleJumpAllowed = false;
        dashAllowed = false;
        wallJumpAllowed = false;
        unlockedSkills = new SerializableDictionary<string, bool>();
    }
}