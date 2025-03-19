using UnityEngine;
using UnityEngine.Events;

namespace GameStateManagement 
{
    [CreateAssetMenu(fileName = "GameStateChannel", menuName = "Channel/Game State Channel")] 
    public class GameStateChannelSO : ScriptableObject
    {
        public UnityAction<GameStateSO> onStateTransitionRequested;

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
    }
}