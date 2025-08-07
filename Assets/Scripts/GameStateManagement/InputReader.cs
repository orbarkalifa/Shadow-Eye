using UnityEngine.SceneManagement;

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

        private InputSystem_Actions inputActions; 

        private void Awake()
        {
            if (FindObjectsByType<InputReader>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
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
            if(inputActions == null)
                return;
            inputActions.Disable();
            inputActions.UI.Disable();
            
            inputActions.UI.ResumeGame.performed -= OnResumeGamePerformed;
            inputActions.UI.RestartGame.performed -= OnRestartGamePerformed;
            inputActions.Player.Menu.performed -= OnOpenMenuPerformed;
        }

        // --- Input Action Event Handlers ---

        private void OnOpenMenuPerformed(InputAction.CallbackContext context)
        {
            if (beacon != null && beacon.gameStateChannel != null && menuState != null && inGameState != null)
            {
                if (beacon.gameStateChannel.GetCurrentGameState() == inGameState) 
                {
                    beacon.gameStateChannel.RaiseStateTransitionRequest(menuState);
                }
                else if (beacon.gameStateChannel.GetCurrentGameState() == menuState)
                {
                    beacon.gameStateChannel.RaiseStateTransitionRequest(inGameState);
                }
            }
            else
            {
                Debug.LogError("InputReader: GSManager component not found on the same GameObject!");
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
            Debug.Log("OnStartGameButton");
            if (beacon != null && beacon.gameStateChannel != null && startGameState != null)
            {
                beacon.uiChannel.ChangeLevel("IntroLevel", inGameState);
                GSManager.Instance.EnableTutorials();
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
                RetryManager.Instance.AttemptRetry();
            }
        }
        
        public void OnExitButton() 
        {
            if (beacon != null && beacon.gameStateChannel != null && startGameState != null)
            {
                beacon.uiChannel.ChangeLevel("startScene", startGameState);   
            }
        }

        public void OnCloseGameButton()
        {
            Debug.Log("OnCloseGameButton called. Quitting game...");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif

        }
    }
}