using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameStateManagement 
{
    [CreateAssetMenu(fileName = "GameStateChannel", menuName = "Channel/Game State Channel")] 
    
    public class GameStateChannelSO : ScriptableObject
    {
        public UnityAction<GameStateSO> onStateTransitionRequested;
        public Func<GameStateSO> GetCurrentGameState;
        public Func<string, GameStateSO> GetGameStateByName;
        

        public void RaiseStateTransitionRequest(GameStateSO nextStateSo)
        {
            if (onStateTransitionRequested != null)
            {
                onStateTransitionRequested.Invoke(nextStateSo);
            }
            else
            {
                Debug.LogWarning("State Transition Requested but no listeners are subscribed to the channel.");
            }
        }
        public GameStateSO OnGetCurrentGameState()
        {
            return GetCurrentGameState?.Invoke();
        }

        public GameStateSO OnGetGameStateByName(string name)
        {
            return GetGameStateByName?.Invoke(name);
        }
    }
    
}