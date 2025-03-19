using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateChannel : ScriptableObject
{
    public Action<GameState> StateEnter;
    public Func<GameState> GetCurrentState;
    public event Action OnMenuClicked;
    public void MenuClicked()
    {
        OnMenuClicked?.Invoke();
    }
    
    public void StateEntered(GameState gameState)
    {
        StateEnter?.Invoke(gameState);
    }
    

    public GameState GetCurrentGameState()
    {
        return GetCurrentState?.Invoke();
    }
}
