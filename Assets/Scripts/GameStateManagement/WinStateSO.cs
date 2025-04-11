namespace GameStateManagement

{
    using UnityEngine;
    [CreateAssetMenu(fileName = "StartGameStateSO", menuName = "GameState/WinGame")] 
    public class WinGameStateSo:GameStateSO
    {
        public override void EnterState()
        {
            base.EnterState();
            Time.timeScale = 0;
            
        }

        public override void ExitState()
        {
            
            base.ExitState();
            Time.timeScale = 1;
        }
    }
}