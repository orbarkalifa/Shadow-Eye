using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

namespace GameStateManagement
{
    public class GSManager : MonoBehaviour
    {
        public BeaconSO beacon;
        public GameStateSO currentState;
        public GameStateSO startGameState;
        public GameStateSO inGameState;
        public GameStateSO menuState;
        public GameStateSO gameOverState;
        public GameStateSO gameWinState;
        
        private UIManager uiManager;
        
        public bool tutorialsEnabled = true;

        private readonly Dictionary<string, GameStateSO> stateLookup = new Dictionary<string, GameStateSO>();
        public static GSManager Instance { get; private set; }
        

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
 
            if (FindObjectsByType<GSManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            EnableTutorials();
            DontDestroyOnLoad(gameObject);
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("GSManager: UIManager not found in scene!");
            }

            beacon.gameStateChannel.GetCurrentGameState += GetCurrentState;
            beacon.gameStateChannel.GetGameStateByName += GetStateByName;
            initializeUIListeners();
        }
        
        void Start()
        {
            populateStateLookup();
            tutorialsEnabled = PlayerPrefs.GetInt("TutorialsEnabled", 1) == 1;

            if (currentState == null)
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                if (currentSceneName == "IntroLevel") 
                {
                    Debug.LogWarning($"GSManager started in a gameplay scene ({currentSceneName}) without a state. Forcing InGameState.");
                    transitionToState(inGameState);
                }
                else
                {
                    transitionToState(startGameState);
                }
            }
        }

        private GameStateSO GetCurrentState()
        {
            return currentState;
        }
        
        private void OnEnable()
        {
            if (beacon != null && beacon.gameStateChannel != null)
            {
                beacon.gameStateChannel.onStateTransitionRequested += transitionToState;
            }
            else
            {
                Debug.LogError("Channel Beacon or GameStateChannel is not assigned in GameStateManager!");
            }
        }

        private void OnDisable()
        {
            if (beacon != null && beacon.gameStateChannel != null)
            {
                beacon.gameStateChannel.onStateTransitionRequested -= transitionToState;
            }
        }
        
        public void DisableTutorials()
        {
            tutorialsEnabled = false;
            PlayerPrefs.SetInt("TutorialsEnabled", 0);
            
        }

        public void EnableTutorials()
        {
            tutorialsEnabled = true;
            PlayerPrefs.SetInt("TutorialsEnabled", 1);
        }

        

        void Update()
        {
            if (currentState != null)
            {
                currentState.UpdateState();
            }
        }

        private void transitionToState(GameStateSO nextState)
        {
            if (currentState == nextState) return;

            if (currentState != null)
            {
                currentState.ExitState();
            }
            currentState = nextState;
            currentState.EnterState();
        }

        public void RequestStartGame() => beacon?.gameStateChannel?.RaiseStateTransitionRequest(startGameState);
        public void RequestInGame() => beacon?.gameStateChannel?.RaiseStateTransitionRequest(inGameState);
        public void RequestMenu() => beacon?.gameStateChannel?.RaiseStateTransitionRequest(menuState);
        public void RequestGameOver() => beacon?.gameStateChannel?.RaiseStateTransitionRequest(gameOverState);

        public GameStateSO GetStateByName(string stateName)
        {
            if (stateLookup.TryGetValue(stateName, out var stateByName))
            {
                return stateByName;
            }
            else
            {
                Debug.LogError($"GSManager: State with name '{stateName}' not found!");
                return null;
            }
        }
        
        private void populateStateLookup()
        {
            stateLookup.Clear();
            if (startGameState != null) stateLookup.Add(startGameState.stateName, startGameState);
            if (inGameState != null) stateLookup.Add(inGameState.stateName, inGameState);
            if (menuState != null) stateLookup.Add(menuState.stateName, menuState);
            if (gameOverState != null) stateLookup.Add(gameOverState.stateName, gameOverState);
            if (gameWinState != null) stateLookup.Add(gameWinState.stateName, gameWinState);
        }
        
        private void initializeUIListeners()
        {
            if (uiManager == null)
            {
                Debug.LogError("GSManager: UIManager instance is null. Cannot initialize UI listeners.");
                return;
            }

            if (startGameState != null)
            {
                startGameState.onEnterState.RemoveAllListeners();
                startGameState.onEnterState.AddListener(uiManager.ShowStartMenuPanel);
            }

            if (inGameState != null)
            {
                inGameState.onEnterState.RemoveAllListeners();
                inGameState.onEnterState.AddListener(uiManager.ShowInGameHUDPanel);
            }

            if (menuState != null)
            {
                menuState.onEnterState.RemoveAllListeners();
                menuState.onEnterState.AddListener(uiManager.ShowPauseMenuPanel);
            }

            if (gameOverState != null)
            {
                gameOverState.onEnterState.RemoveAllListeners();
                gameOverState.onEnterState.AddListener(uiManager.ShowGameOverPanel);
            }

            if(gameWinState != null)
            {
                gameWinState.onEnterState.RemoveAllListeners();
                gameWinState.onEnterState.AddListener(uiManager.ShowWinGameHUDPanel);
            }
        }
        
    }
}