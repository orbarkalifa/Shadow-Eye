using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "StartGameStateSO", menuName = "GameState/StartGame")] 
    public class StartGameStateSO : GameStateSO
    {
        public override void EnterState()
        {
            base.EnterState();
            Time.timeScale = 0;
            // enable your Start Menu UI GameObject here using onEnterState UnityEvent in the Inspector.
        }

        public override void ExitState()
        {
            
            base.ExitState();
            Time.timeScale = 1;
            // disable Start Menu UI GameObject here using onExitState UnityEvent in the Inspector.
        }
    }
}