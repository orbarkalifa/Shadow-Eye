using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private List<GameState> states = new();
    [SerializeField] private GameState currentState;
    [SerializeField] private GameState defaultState;
    private GameStateChannel gameStateChannel;

    void Start()
    {
        
        
        var beacon = FindObjectOfType<Beacon>();
        gameStateChannel = beacon.gameStateChannel;
        gameStateChannel.StateEnter += StateEnter;
        gameStateChannel.GetCurrentState += GetCurrentState;

        if (defaultState == null)
        {
            Debug.LogError("Default state is not assigned! Assign it in the Inspector.");
            return;
        }
        if (gameStateChannel == null)
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
            gameStateChannel.StateEntered(defaultState);
        }

        SceneManager.sceneLoaded += AnnounceStateOnSceneLoaded;
    }


    private void AnnounceStateOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var state = currentState;
        if (state == null) 
        {
            state = states.FirstOrDefault(x => x.isCurrentState);
        }
        gameStateChannel.StateEntered(state);
    }

    private GameState GetCurrentState()
    {
        return currentState;
    }

    private void StateEnter(GameState state)
    {
        currentState = state;
    }

    private void OnDestroy()
    {
        gameStateChannel.StateEnter -= StateEnter;
        gameStateChannel.GetCurrentState -= GetCurrentState;
    }
}