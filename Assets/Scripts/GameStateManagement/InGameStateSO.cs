using UnityEngine;

namespace GameStateManagement
{
    public class InGameState : GameStateSO
    {
        public override void EnterState()
        {
            base.EnterState();
            // State-specific logic when entering InGame state
            Time.timeScale = 1f; // Ensure time is running in-game
            Debug.Log("InGameState: EnterState - Starting Game, Enabling In-Game UI, Resuming Time");
            // Enable in-game UI, game logic, player input, etc. using onEnterState UnityEvent in the Inspector.
        }

        public override void ExitState()
        {
            base.ExitState();
            // State-specific logic when exiting InGame state
            Debug.Log("InGameState: ExitState - Disabling In-Game UI (Placeholder)");
            // Disable in-game UI, game logic, player input (if needed for other states) using onExitState UnityEvent in the Inspector.
        }

        public override void UpdateState()
        {
            base.UpdateState();
            // In-game specific update logic (if any)
            // Example: Check for game over conditions, etc.
            // Most in-game logic will be in other scripts (PlayerController, Enemy AI, etc.)
        }
    }    
}
