using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranstionInGame : TransitionBase

{
    private GameStateChannel gameStateChannel;
    private bool menuPressed;
    protected override void Awake()
    {
        base.Awake();

        // Initialize Input Actions
        // Get the GameStateChannel
        gameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;
        //    gameStateChannel.StateEnter += StateEnter;
         // Optional, if additional handling is required
    }
    public void OnResumeClicked()
    {
        menuPressed = true; // Optionally, synchronize state with GameStateChannel
    }
    public override bool ShouldTransition()
    {
        bool baseCheck = base.ShouldTransition();         // Is this the current state?
        bool canTransition = menuPressed;
        // Reset the pressed flag for next time
        menuPressed = false;

        return baseCheck && canTransition;
    }
    private void OnDestroy()
    {
        // Cleanup to avoid memory leaks
        //       gameStateChannel.StateEnter -= StateEnter;
        gameStateChannel.OnMenuClicked -= OnResumeClicked;
    }
}
