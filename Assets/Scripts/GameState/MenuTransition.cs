using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuTransition : TransitionBase
{
    private bool menuPressed;
    private bool canMenu;
    private GameStateChannel gameStateChannel;
    private InputSystem_Actions inputActions;

    protected override void Awake()
    {
        base.Awake();

        // Initialize Input Actions
        inputActions = new InputSystem_Actions();
        // Get the GameStateChannel
        gameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;
        //gameStateChannel.StateEnter += StateEnter;
        gameStateChannel.OnMenuClicked += PressedMenu; // Optional, if additional handling is required
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void StateEnter(GameState state)
    {
        Debug.Log("Menu Changed to true");
        canMenu = state.stateSO.canMenu;
    }
    
    private void PressedMenu()
    {
        menuPressed = true; // Optionally, synchronize state with GameStateChannel
    }

    public override bool ShouldTransition()
    {
        var canTransition = menuPressed && canMenu;
        menuPressed = false; // Reset the flag after the check
        return base.ShouldTransition() && canTransition;
    }

    private void OnDestroy()
    {
        // Cleanup to avoid memory leaks
        inputActions.Dispose();
        gameStateChannel.StateEnter -= StateEnter;
        gameStateChannel.OnMenuClicked -= PressedMenu;
    }
}