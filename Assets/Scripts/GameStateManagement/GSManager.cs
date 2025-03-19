using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState currentState;
    public BeaconSO beacon;

    public GameState startGameState;
    public GameState inGameState;
    public GameState menuState;
    public GameState gameOverState;

    private void OnEnable()
    {
        if (beacon != null && beacon.gameStateChannel != null)
        {
            beacon.gameStateChannel.onStateTransitionRequested += TransitionToState; // Access channel through beacon
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
            beacon.gameStateChannel.onStateTransitionRequested -= TransitionToState; // Access channel through beacon
        }
    }

        void Start()
        {
            if (startGameState != null)
            {
                TransitionToState(startGameState); // Start with the StartGame state
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
                currentState.UpdateState(); // Call UpdateState on the current state
            }
        }

        public void TransitionToState(GameState nextState)
        {
            if (currentState == nextState) return; // Prevent transitioning to the same state

            if (currentState != null)
            {
                currentState.ExitState(); // Exit the current state
            }

            currentState = nextState; // Set the new state
            currentState.EnterState(); // Enter the new state
        }

        // Helper methods to request transitions from other scripts (optional but convenient)
        public void RequestStartGame() => beacon?.gameStateChannel?.RaiseStateTransitionRequest(startGameState); // Access channel through beacon
        public void RequestInGame() => beacon?.gameStateChannel?.RaiseStateTransitionRequest(inGameState); // Access channel through beacon
        public void RequestMenu() => beacon?.gameStateChannel?.RaiseStateTransitionRequest(menuState); // Access channel through beacon
        public void RequestGameOver() => beacon?.gameStateChannel?.RaiseStateTransitionRequest(gameOverState); // Access channel through beacon

    }
}