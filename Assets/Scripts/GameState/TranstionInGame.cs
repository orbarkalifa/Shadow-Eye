using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranstionInGame : TransitionBase

{
    private GameStateSO m_GameStateSo;
    private bool menuPressed;
    protected override void Awake()
    {
        base.Awake();

        // Initialize Input Actions
        // Get the GameStateChannel
        m_GameStateSo = FindObjectOfType<Beacon>().m_GameStateSo;
        //    gameStateChannel.StateEnter += StateEnter;
         // Optional, if additional handling is required
    }
    public void OnResumeClicked()
    {
        menuPressed = true; // Optionally, synchronize state with GameStateChannel
    }
    public override bool ShouldTransition()
    {
        bool canTransition = menuPressed;
        // Reset the pressed flag for next time
        menuPressed = false;

        return canTransition;
    }
    private void OnDestroy()
    {
        // Cleanup to avoid memory leaks
        //       gameStateChannel.StateEnter -= StateEnter;
        m_GameStateSo.OnMenuClicked -= OnResumeClicked;
    }
}
