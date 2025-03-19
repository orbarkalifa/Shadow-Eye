namespace GameStateManagement
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "MenuStateSO", menuName = "GameStateSO/Menu")]
    public class MenuState : GameStateSO
    {
        public override void EnterState()
        {
            base.EnterState();
            Time.timeScale = 0f; // Pause time in menu
            Debug.Log("MenuState: EnterState - Pausing Game, Enabling Menu UI");
            // Enable menu UI, disable in-game input (if needed) using onEnterState UnityEvent in the Inspector.
        }

        public override void ExitState()
        {
            base.ExitState();
            Time.timeScale = 1f; // Resume time when exiting menu (if going back to InGame)
            Debug.Log("MenuState: ExitState - Disabling Menu UI, Resuming Time (if returning to InGame)");
            // Disable menu UI using onExitState UnityEvent in the Inspector.
        }
    }
}