using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public stateSO stateSO;
    public bool isCurrentState = false;
    private GameState nextState = null;
    public GameState previousState = null;
    private List<TransitionBase> transitions = new();
    public bool wasTransitionInto = false;
    public bool inTransition = false;

    private GameStateChannel gameStateChannel;

    private void Start()
    {
        gameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;

        foreach (var transition in GetComponentsInChildren<TransitionBase>())
        {
            transitions.Add(transition);
        }
    }

    private void Update()
    {
        if (!isCurrentState)
            return;

        nextState = null;
        foreach (var transition in transitions.Where(x => x.ShouldTransition()))
        {
            if (transition.TargetState != null)
            {
                nextState = transition.TargetState;
            }
            break;
        }

        if (!inTransition && nextState != null)
        {
            inTransition = true;
            StateExit(nextState);
            inTransition = false;
        }

        if (wasTransitionInto)
        {
            wasTransitionInto = false;
        }
    }

    private void StateEnter(GameState previous)
    {
        previousState = previous;
        gameStateChannel.StateEnter(this);
        isCurrentState = true;
        wasTransitionInto = true;
    }

    private void StateExit(GameState next)
    {
        isCurrentState = false;
        gameStateChannel.StateExited(this);
        next.StateEnter(this);
    }
}