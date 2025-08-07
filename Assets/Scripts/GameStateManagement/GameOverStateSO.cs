namespace GameStateManagement
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameOverStateSO", menuName = "GameState/GameOver")]
    public class GameOverStateSO : GameStateSO
    {
        public override void EnterState()
        {
            base.EnterState();
            Time.timeScale = 0f; 
            Debug.Log("GameOverState: EnterState - Game Over, Pausing Game, Enabling Game Over UI");
        }

        public override void ExitState()
        {
            base.ExitState();
            Time.timeScale = 1f; 
            Debug.Log("GameOverState: ExitState - Disabling Game Over UI (Placeholder)");
        }
    }
}