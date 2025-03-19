namespace GameStateManagement
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "StartGameStateSO", menuName = "GameState/StartGame")] // Added [CreateAssetMenu] and corrected class name to GameStateSO
    public class StartGameStateSO : GameStateSO
    {
        public override void EnterState()
        {
            base.EnterState();
            // State-specific logic when entering StartGame state
            // Example: Enable Start Menu UI
            Debug.Log("StartGameState: EnterState - Enabling Start Menu UI (Placeholder)");
            // You would typically enable your Start Menu UI GameObject here using onEnterState UnityEvent in the Inspector.
        }

        public override void ExitState()
        {
            base.ExitState();
            // State-specific logic when exiting StartGame state
            // Example: Disable Start Menu UI
            Debug.Log("StartGameState: ExitState - Disabling Start Menu UI (Placeholder)");
            // You would typically disable your Start Menu UI GameObject here using onExitState UnityEvent in the Inspector.
        }
    }
}