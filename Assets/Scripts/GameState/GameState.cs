using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public stateSO stateSO;
    public bool isCurrentState = false;
    [SerializeField] private GameState nextState;
    public GameState previousState;
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

    private void StateEnter(GameState previous)
    {
        Debug.Log($"Entering state: {stateSO.m_States}, from previous: {previous.stateSO.m_States}");
        previousState = previous;
        gameStateChannel.StateEnter?.Invoke(this);
        isCurrentState = true;
    }

    private void StateExit(GameState previous)
    {
        Debug.Log($"Exiting state: {stateSO.m_States}, to next: {previous.stateSO.m_States}");
        previousState = previous;
        isCurrentState = false;
        gameStateChannel.StateExit?.Invoke(this);
    }

}