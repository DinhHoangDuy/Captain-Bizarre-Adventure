using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [SerializeField] private String mapSceneName;
    [SerializeField] private Button exitSaveSlotsMenuButton;
    private SaveSlot[] saveSlots;

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }
    void Start()
    {
        exitSaveSlotsMenuButton.onClick.AddListener(GetComponentInParent<MainMenu>().BackToMainMenu);
    }

    public void ActivateMenu()
    {
        Dictionary<string, GameData> gameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // bool anySlotHasData = false;
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            gameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);

            // if (profileData != null)
            // {
            //     anySlotHasData = true;
            // }
        }
         
        //====== Choose the Slot automatically ======
        exitSaveSlotsMenuButton.Select();

        // // Case 1: if there's no data in any slot, select the first slot
        // if (!anySlotHasData)
        // {
        //     Debug.Log("No data found in any slot. Selecting the first slot.");
        //     saveSlots[0].GetComponent<Button>().Select();
        // }
        // // Case 2: if there's at least one slot with data...
        // else
        // {
        //     // Step 1: remember the first slot with data, then count the number of slots with data
        //     SaveSlot firstSlotWithData = null;
        //     int numberOfSlotsWithData = 0;
        //     foreach (SaveSlot saveSlot in saveSlots)
        //     {
        //         if (saveSlot.profileHasData)
        //         {
        //             if (firstSlotWithData == null)
        //             {
        //                 firstSlotWithData = saveSlot;
        //             }
        //             numberOfSlotsWithData++;
        //         }
        //     }

        //     // Step 2
        //     // Case 1: if there's only one slot with data, select it
        //     if (numberOfSlotsWithData == 1)
        //     {
        //         Debug.Log("Only one slot has data. Selecting this slot.");
        //         firstSlotWithData.GetComponent<Button>().Select();
        //     }
        //     // Case 2: if there's more than one slot with data, select the most recently updated slot
        //     else if (numberOfSlotsWithData > 1)
        //     {
        //         string mostRecentlyUpdatedProfileID = DataPersistenceManager.instance.GetMostRecentlyUpdatedProfileID();
        //         foreach (SaveSlot saveSlot in saveSlots)
        //         {
        //             if (saveSlot.GetProfileId() == mostRecentlyUpdatedProfileID)
        //             {
        //                 Debug.Log("Most recently updated slot found" + saveSlot.GetProfileId() + ". Selecting this slot.");
        //                 saveSlot.GetComponent<Button>().Select();
        //                 break;
        //             }
        //         }
        //     }
        //     else // Default: select the first slot because of an unexpected error which should not happen.
        //     {
        //         Debug.Log("Default: No data found in any slot. Selecting the first slot.");
        //         saveSlots[0].GetComponent<Button>().Select();
        //     }
        // }
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
        // if the profile has data, load the game. Otherwise, start a new game
        if(!saveSlot.profileHasData)
        {
            DataPersistenceManager.instance.NewGame();
        }

        // SceneManager.LoadScene(mapSceneName);
        MainMenu.instance.LoadGameAnimation(mapSceneName);
    }

    // TODO: implement the "Delete" button

    
}
