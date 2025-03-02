using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public stateSO stateSO;
    [SerializeField] public GameState nextState;
    private List<TransitionBase> transitions = new();
    public bool wasTransitionInto = false;
    public bool inTransition = false;
    private GameStateSO m_GameStateSo;

    private void Awake()
    {
        m_GameStateSo = FindObjectOfType<Beacon>().GameStateSo;
        foreach (var transition in GetComponentsInChildren<TransitionBase>())
        {
            transitions.Add(transition);
        }
    }

    private void Update()
    { 
       Debug.Log($"game state is  current {m_GameStateSo.GetCurrentGameState()} checking script{this} state :{CheckifCurrent()}");
        if (!CheckifCurrent())
            return;
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

        if (!inTransition &&!wasTransitionInto && nextState != null)
        {
            inTransition = true;
            StateEnter(nextState);
            nextState = null;
            inTransition = false;
        }

        if (wasTransitionInto)
        {
            wasTransitionInto = false;
        }


    }
    public bool CheckifCurrent()
    {
        return m_GameStateSo.GetCurrentGameState() == this;;
    }
    public void StateEnter(GameState next)
    {
        wasTransitionInto = true;
        Debug.Log($"game state is entered {nextState.CheckifCurrent()}");
        m_GameStateSo.StateEntered(next);
    }



}