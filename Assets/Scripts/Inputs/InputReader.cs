using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

// Scriptable object allows you to create an asset rather than just a script
[CreateAssetMenu(fileName = "Input Reader", menuName = "Inputs/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private Controls controls;
    public event Action<bool> PrimaryFireEvent;
    public event Action<Vector2> MoveEvent;
    private void OnEnable() {
        // create an instance of our controls and tell the input system to use it
        if (controls == null) {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed) {
            PrimaryFireEvent?.Invoke(true);
        } else if (context.canceled) {
            PrimaryFireEvent?.Invoke(false);
        }
    }
}
