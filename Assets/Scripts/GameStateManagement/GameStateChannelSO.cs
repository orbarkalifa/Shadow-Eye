using UnityEngine;
using UnityEngine.Events;

namespace GameStateManagement
{
    public class GameStateChannel : ScriptableObject
    {
        public UnityAction<GameState> onStateTransitionRequested;

        public void RaiseStateTransitionRequest(GameState nextState)
        {
            if (onStateTransitionRequested != null)
            {
                onStateTransitionRequested.Invoke(nextState);
            }
            else
            {
                Debug.LogWarning("State Transition Requested but no listeners are subscribed to the channel.");
            }
        }
    }
}