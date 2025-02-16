using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput inputActions;
    public static InputManager instance;

    internal bool attackInputTriggered = false;
    internal bool ultimateInputTriggered = false;
    internal bool healInputTriggered = false;
    internal bool expansionChipPanelInputTriggered = false;
    internal bool interactionInputTriggered = false;

    void Awake()
    {
        inputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Performed Method is called when the button is pressed
        inputActions.Player.Heal.performed += ctx => healInputTriggered = true;
        inputActions.Player.ExpansionChipPanel.performed += ctx => expansionChipPanelInputTriggered = true;
        inputActions.Player.Interact.performed += ctx => interactionInputTriggered = true;

        inputActions.Player.Attack.performed += ctx => attackInputTriggered = true;
        inputActions.Player.Ultimate.performed += ctx => ultimateInputTriggered = true;

        // Cancel Method is called when the button is released
        inputActions.Player.Heal.canceled += ctx => healInputTriggered = false;
        inputActions.Player.ExpansionChipPanel.canceled += ctx => expansionChipPanelInputTriggered = false;
        inputActions.Player.Interact.canceled += ctx => interactionInputTriggered = false;

        inputActions.Player.Attack.canceled += ctx => attackInputTriggered = false;
        inputActions.Player.Ultimate.canceled += ctx => ultimateInputTriggered = false;

    }
}
