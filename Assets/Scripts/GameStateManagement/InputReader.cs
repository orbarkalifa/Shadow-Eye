namespace GameStateManagement
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputReader : MonoBehaviour
    {
        public BeaconSO beacon;
        public GameStateSO inGameState;
        public GameStateSO menuState;
        public GameStateSO startGameState;

        public GSManager gsManager;
        private InputSystem_Actions inputActions; 

        private void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.UI.Enable();

            // Gameplay Actions
            inputActions.Player.Menu.performed += OnOpenMenuPerformed;

            // UI Actions
            inputActions.UI.ResumeGame.performed += OnResumeGamePerformed;
            inputActions.UI.RestartGame.performed += OnRestartGamePerformed;
        }

        private void OnDisable()
        {
            inputActions.Disable();
            inputActions.UI.Disable();

            inputActions.UI.ResumeGame.performed -= OnResumeGamePerformed;
            inputActions.UI.RestartGame.performed -= OnRestartGamePerformed;
            inputActions.Player.Menu.performed -= OnOpenMenuPerformed;

        }

        // --- Input Action Event Handlers ---

        private void OnStartGamePerformed(InputAction.CallbackContext context)
        {
            if (beacon != null && beacon.gameStateChannel != null && startGameState != null)
            {
                beacon.gameStateChannel.RaiseStateTransitionRequest(inGameState); 
            }
        }

        private void OnOpenMenuPerformed(InputAction.CallbackContext context)
        {
            if (beacon != null && beacon.gameStateChannel != null && menuState != null && inGameState != null) // Added check for inGameState to be not null as well, for safety
            {
                if (gsManager != null)
                {
                    if (gsManager.currentState == inGameState) // If currently in InGame state, open Menu
                    {
                        beacon.gameStateChannel.RaiseStateTransitionRequest(menuState);
                    }
                    else if (gsManager.currentState == menuState) // If currently in Menu state, close Menu and go back to InGame
                    {
                        beacon.gameStateChannel.RaiseStateTransitionRequest(inGameState);
                    }
                    // If in other states (like StartGame or GameOver), you might choose to do nothing or handle differently.
                    // For example, you might want to prevent opening the menu in GameOver state.
                }
                else
                {
                    Debug.LogError("InputReader: GSManager component not found on the same GameObject!");
                }
            }
        }

        private void OnResumeGamePerformed(InputAction.CallbackContext context)
        {
            if (beacon != null && beacon.gameStateChannel != null && inGameState != null)
            {
                beacon.gameStateChannel.RaiseStateTransitionRequest(inGameState);
            }
        }

        private void OnRestartGamePerformed(InputAction.CallbackContext context)
        {
            if (beacon != null && beacon.gameStateChannel != null && inGameState != null)
            {
                beacon.gameStateChannel.RaiseStateTransitionRequest(inGameState); 
            }
        }


        public void OnStartGameButton()
        {
            if (beacon != null && beacon.gameStateChannel != null && startGameState != null)
            {
                beacon.gameStateChannel.RaiseStateTransitionRequest(inGameState); // Go to InGame state
            }
        }

        public void OnResumeGameButton() 
        {
            if (beacon != null && beacon.gameStateChannel != null && inGameState != null)
            {
                beacon.gameStateChannel.RaiseStateTransitionRequest(inGameState); // Resume Game
            }
        }

        public void OnRestartGameButton() 
        {
            if (beacon != null && beacon.gameStateChannel != null && inGameState != null)
            {
                beacon.gameStateChannel.RaiseStateTransitionRequest(inGameState); // Restart Game
            }
        }
    }
}