using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public stateSO stateSO;
    public bool isCurrentState = false;
    [SerializeField] public GameState nextState;
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
        
        if (gameStateChannel.GetCurrentGameState() != this)
            return;
        isCurrentState = true;
        nextState = null;
        foreach (var transition in transitions.Where(x => x.ShouldTransition()))
        {
            if (transition.TargetState != null)
            {
                nextState = transition.TargetState;
                Debug.Log($"found target {transition.TargetState}");
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
    private void StateEnter(GameState nextState)
    {
        Debug.Log($"Entering state: {nextState}, from previous: {gameStateChannel.GetCurrentState}");
        gameStateChannel.StateEntered(nextState);
        isCurrentState = true;
        wasTransitionInto = true;
    }

    private void StateExit(GameState next)
    {
        isCurrentState = false;
        gameStateChannel.StateExited(this);
        next.StateEnter(next);
    }

}