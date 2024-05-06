using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerConsoleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    // [SerializeField] private GameObject expansionChipPanel;
    public static bool consolePanelActive = false;

    private PlayerInput playerInput;
    private bool consolePressed = false;

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
        // expansionChipPanel.SetActive(false);
    }

    private void Update()
    {
        consolePressed = playerInput.Game.Inventory.triggered && PauseMenu.isPaused == false;

        if (consolePressed)
        {
            ToggleConsolePanel();
        }
    }

    private void ToggleConsolePanel()
    {
        if(!consolePanelActive)
        {
            PlatformerMovement2D.blocked = true;
            Time.timeScale = 0f;
            inventoryPanel.SetActive(true);
            // expansionChipPanel.SetActive(false);
            consolePanelActive = true;
        }
        else
        {
            PlatformerMovement2D.blocked = false;
            Time.timeScale = 1f;
            inventoryPanel.SetActive(false);
            // expansionChipPanel.SetActive(false);
            consolePanelActive = false;
        }
    }
}
