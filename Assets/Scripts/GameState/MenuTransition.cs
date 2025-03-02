using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuTransition : TransitionBase
{
    private bool UserPressedMenu;
    public bool canMenu = false;
    private GameStateSO m_GameStateSo;

    protected override void Awake()
    {
        base.Awake();

        // Initialize Input Actions
        // Get the GameStateChannel
        m_GameStateSo = FindObjectOfType<Beacon>().GameStateSo;
        m_GameStateSo.OnMenuClicked += CheckIfMenuWasPressed; // Optional, if additional handling is required
    }
    
    private void CheckIfMenuWasPressed()
    {
        if(!sourceState.CheckifCurrent())return;//Fail safe 
        UserPressedMenu = true; // Optionally, synchronize state with GameStateChannel
    }
    public override bool ShouldTransition()
    {
        if(!sourceState.CheckifCurrent()) return false;// Fail safe
        bool canTransition = (UserPressedMenu && m_GameStateSo.GetCurrentGameState().stateSO.canMenu);    // Did user press menu & current state allows it?
        UserPressedMenu = false; // Is this the current state?
        // Debug log the result
        Debug.Log($"source {sourceState} to {targetState} , {canTransition}");
        // Reset the pressed flag for next time
        return canTransition;
    }

    private void OnDestroy()
    {
        m_GameStateSo.OnMenuClicked -= CheckIfMenuWasPressed;
    }
}