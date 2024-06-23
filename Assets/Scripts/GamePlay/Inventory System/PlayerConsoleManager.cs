using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerConsoleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject expansionChipPanel;
    public static bool consolePanelActive = false;
    private bool inventoryPanelActive = false;
    private bool expansionChipPanelActive = false;

    private PlayerInput playerInput;
    private bool inventoryPressed = false;
    private bool expansionChipPressed = false;

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Game.Inventory.Enable();
    }

    private void OnDisable()
    {
        playerInput.Game.Inventory.Disable();
    }

    private void Start()
    {
        inventoryPanel.SetActive(false);
        expansionChipPanel.SetActive(false);
    }

    private void Update()
    {
        inventoryPressed = playerInput.Game.Inventory.triggered && !PauseMenu.isPaused;
        // TODO: Fix this error: it doenst reconize the "playerInput.Game.ExpansionChipPanel.triggered" condition
        expansionChipPressed = Input.GetKeyDown(KeyCode.R) && !PauseMenu.isPaused;
    
        if (inventoryPressed)
        {
            Debug.Log("Inventory Panel Method Trigger");
            ToggleInventoryPanel();
        }
        if (expansionChipPressed)
        {
            Debug.Log("Expansion Chip Panel Method Trigger");
            ToggleExpansionChipPanel();
        }
    }

    private void ToggleInventoryPanel()
    {
        // If the inventory panel is currently inactive, activate it and deactivate the expansion chip panel
        if (!inventoryPanelActive)
        {
            inventoryPanelActive = true;
            expansionChipPanelActive = false;
            inventoryPanel.SetActive(true);
            expansionChipPanel.SetActive(false);

            GetComponent<InventoryManager>().DeselectAllSlots();
        }
        else
        {
            // If the inventory panel is already active, just deactivate it
            inventoryPanelActive = false;
            inventoryPanel.SetActive(false);
        }

        // Optionally, handle what happens when the console panel becomes active or inactive
        HandleConsolePanelState();
    }

    private void ToggleExpansionChipPanel()
{
    // If the expansion chip panel is currently inactive, activate it and deactivate the inventory panel
    if (!expansionChipPanelActive)
    {
        expansionChipPanelActive = true;
        inventoryPanelActive = false;
        expansionChipPanel.SetActive(true);
        inventoryPanel.SetActive(false);

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
        consolePanelActive = inventoryPanelActive || expansionChipPanelActive;

        // Here you can add any additional logic that should occur when the console panel's state changes
        // For example, pausing the game, changing the player's ability to move, etc.
        PlatformerMovement2D.instance.blocked = consolePanelActive;
    }

}
