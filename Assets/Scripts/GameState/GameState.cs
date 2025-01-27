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
    }

    private void StateEnter(GameState nextState)
    {
        Debug.Log($"Entering state: {stateSO.m_States}, from previous: {nextState.stateSO.m_States}");
        gameStateChannel.StateEntered(this.nextState);
        isCurrentState = true;
    }

    private void StateExit(GameState previous)
    {
        Debug.Log($"Exiting state: {stateSO.m_States}, to next: {previous.stateSO.m_States}");
        isCurrentState = false;
        gameStateChannel.StateExited(previous);
    }

}