using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerConsoleManager : MonoBehaviour
{
    // [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject expansionChipPanel;
    public static bool consolePanelActive = false;
    // private bool inventoryPanelActive = false;
    private bool expansionChipPanelActive = false;

    private PlayerInput playerInput;
    // private bool inventoryPressed = false;
    private bool expansionChipPressed = false;

    private void OnEnable()
    {
        playerInput = new PlayerInput();

    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        // inventoryPanel.SetActive(false);
        expansionChipPanel.SetActive(false);
    }

    private void Update()
    {
        // expansionChipPressed = playerInput.Player.ExpansionChipPanel.triggered && !PauseMenu.isPaused;
        expansionChipPressed = Input.GetKeyDown(KeyCode.R) && !PauseMenu.isPaused;
    
        if (expansionChipPressed)
        {
            Debug.Log("Expansion Chip Panel Method Trigger");
            ToggleExpansionChipPanel();
        }
    }


    private void ToggleExpansionChipPanel()
{
    if (!expansionChipPanelActive)
    {
        expansionChipPanelActive = true;
        expansionChipPanel.SetActive(true);
        // inventoryPanel.SetActive(false);

        GetComponent<ExpansionChipManager>().DeselectAllSlots();
        GetComponent<ExpansionChipManager>().DeleteDescription();
    }
    else
    {
        // If the expansion chip panel is already active, just deactivate it
        expansionChipPanelActive = false;
        expansionChipPanel.SetActive(false);
    }

    // Optionally, handle what happens when the console panel becomes active or inactive
    HandleConsolePanelState();
}

    private void HandleConsolePanelState()
    {
        // If any panel is active, the console panel is considered active
        consolePanelActive = expansionChipPanelActive;

        // Here you can add any additional logic that should occur when the console panel's state changes
        // For example, pausing the game, changing the player's ability to move, etc.
        // PlatformerMovement2D.instance.inputBlocked = consolePanelActive;
    }

}
