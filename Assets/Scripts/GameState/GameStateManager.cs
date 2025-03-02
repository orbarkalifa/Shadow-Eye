using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public GameState currentState;
    public GameState previousState;
    public GameState defaultState;

    private List<GameState> states = new();
    private GameStateSO m_GameStateSo;

    void Awake()
    {
        var beacon = FindObjectOfType<Beacon>();
        m_GameStateSo = beacon.GameStateSo;
        m_GameStateSo.StateEnter += StateEnter;
        m_GameStateSo.GetCurrentState += GetCurrentState;

        if (defaultState == null)
        {
            Debug.LogError("Default state is not assigned! Assign it in the Inspector.");
            return;
        }
        if (m_GameStateSo == null)
        {
            Debug.LogError("GameStateChannel is not assigned or missing in the scene.");
            return;
        }
        
        foreach (var state in GetComponentsInChildren<GameState>())
        {
            states.Add(state);
        }

        if (currentState == null)
        {
            currentState = defaultState;
            defaultState.StateEnter(defaultState);
        }

        SceneManager.sceneLoaded += AnnounceStateOnSceneLoaded;
    }


    private void AnnounceStateOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var state = currentState;
        if (state == null) 
        {
            state = states.FirstOrDefault(x => x.CheckifCurrent());
        }
        m_GameStateSo.StateEntered(state);
    }

    private GameState GetCurrentState()
    {
        return currentState;
    }

    private void StateEnter(GameState state)
    {
        previousState = currentState;
        currentState = state;
    }

    private void OnDestroy()
    {
        m_GameStateSo.StateEnter -= StateEnter;
        m_GameStateSo.GetCurrentState -= GetCurrentState;
    }
}