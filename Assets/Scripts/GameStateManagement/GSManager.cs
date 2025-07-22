using System;
using System.Collections.Generic;
using UnityEngine;

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
            populateStateLookup();
            beacon.gameStateChannel.GetCurrentGameState += GetCurrentState;
            beacon.gameStateChannel.GetGameStateByName += GetStateByName;
            beacon.uiChannel.InitializeListeners();

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

        void Start()
        {

            tutorialsEnabled = PlayerPrefs.GetInt("TutorialsEnabled", 1) == 1;

            if (startGameState != null)
            {
                transitionToState(startGameState);
            }
            else
            {
                Debug.LogError("Start Game State is not assigned in GameStateManager!");
            }
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
        
    }
}