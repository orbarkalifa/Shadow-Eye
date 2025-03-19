using UnityEngine;
using UnityEngine.Events;

namespace GameStateManagement
{
    public class GameState : ScriptableObject
    {
        public string stateName;
        public UnityEvent onEnterState;
        public UnityEvent onExitState;
        public virtual void EnterState()
        {
            Debug.Log($"Entering State: {stateName}");
            onEnterState?.Invoke();
        }

        public virtual void ExitState()
        {
            Debug.Log($"Exiting State: {stateName}");
            onExitState?.Invoke();
        }

        public virtual void UpdateState()
        {
            // Optional: State-specific update logic (e.g., timers, animations)
        }
    }
}