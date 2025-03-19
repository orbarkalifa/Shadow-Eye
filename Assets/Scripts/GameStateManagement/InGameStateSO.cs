namespace GameStateManagement
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "InGameStateSO", menuName = "GameState/InGame")]
    public class InGameStateSO : GameStateSO
    {
        public override void EnterState()
        {
            base.EnterState();
            Time.timeScale = 1f; 
            Debug.Log("InGameState: EnterState - Starting Game, Enabling In-Game UI, Resuming Time");
            // Enable in-game UI, game logic, player input, etc. using onEnterState UnityEvent in the Inspector.
        }

        public override void ExitState()
        {
            base.ExitState();
            Debug.Log("InGameState: ExitState - Disabling In-Game UI (Placeholder)");
            // Disable in-game UI, game logic, player input (if needed for other states) using onExitState UnityEvent in the Inspector.
        }


    }
}