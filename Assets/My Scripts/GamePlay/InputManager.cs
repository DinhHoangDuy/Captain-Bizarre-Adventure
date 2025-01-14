using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput inputActions;
    public static InputManager instance;

    // Trigger indicator for "button" Triggers
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


        inputActions.Player.Heal.started += ctx => healInputTriggered = true; Debug.Log("Heal Input Triggered");
        inputActions.Player.ExpansionChipPanel.started += ctx => expansionChipPanelInputTriggered = true; Debug.Log("Expansion Chip Panel Input Triggered");
        inputActions.Player.Interact.started += ctx => interactionInputTriggered = true; Debug.Log("Interaction Input Triggered");

        inputActions.Player.Move.performed += ctx =>
        {
            Vector2 moveInput = ctx.ReadValue<Vector2>();
            Debug.Log("Move Input: " + moveInput);
        };
    }

    void Update()
    {

    }
}
