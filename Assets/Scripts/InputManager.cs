using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager: MonoBehaviour
{
    public static InputSystem_Actions InputActions ;

    private void Awake()
    {
        ;
    }

    public void Initialize()
    {
        InputActions.Enable(); // Enable the Input System
    }

    public void Dispose()
    {
        InputActions?.Dispose(); // Clean up when no longer needed
    }

    // Example: Register an action for the Menu button
    public void RegisterMenuAction(System.Action<InputAction.CallbackContext> callback)
    {
        InputActions.Player.Menu.performed += callback;
    }

    public void UnregisterMenuAction(System.Action<InputAction.CallbackContext> callback)
    {
        InputActions.Player.Menu.performed -= callback;
    }
}