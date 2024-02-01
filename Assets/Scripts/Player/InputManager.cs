using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlatformerInputAction platformerInputaction;
    public InputAction horizontalInput;
    public InputAction jumpInput;
    public InputAction dashInput;

    private void Awake()
    {
        platformerInputaction = new PlatformerInputAction();
    }

    private void OnEnable()
    {
        horizontalInput = platformerInputaction.Player.Movement;
        horizontalInput.Enable();

        jumpInput = platformerInputaction.Player.Jump;
        jumpInput.Enable();

        dashInput = platformerInputaction.Player.Dash;
        dashInput.Enable();
    }

    private void OnDisable()
    {
        horizontalInput.Disable();
        jumpInput.Disable();
        dashInput.Disable();
    }
}
