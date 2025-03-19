namespace GameStateManagement
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameOverStateSO", menuName = "GameState/GameOver")]
    public class GameOverStateSO : GameStateSO
    {
        public override void EnterState()
        {
            base.EnterState();
            Time.timeScale = 0f; // Pause time in game over
            Debug.Log("GameOverState: EnterState - Game Over, Pausing Game, Enabling Game Over UI");
            // Enable game over UI using onEnterState UnityEvent in the Inspector.
        }

        public override void ExitState()
        {
            base.ExitState();
            Debug.Log("GameOverState: ExitState - Disabling Game Over UI (Placeholder)");
            // Disable game over UI using onExitState UnityEvent in the Inspector.
        }
    }
}