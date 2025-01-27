using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField]
    private GameObject menuPanel;
    private GameStateChannel gameStateChannel;
    [SerializeField] private GameState menuState;

    private void Awake()
    {
        if (menuState == null)
        {
            Debug.LogError("MenuState is not assigned! Assign it in the Inspector.");
        }

        if (menuPanel == null)
        {
            Debug.LogError("MenuPanel is not assigned! Assign it in the Inspector.");
        }
        
        gameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;
        if (gameStateChannel == null)
        {
            Debug.LogError("gameStateChannel not found in the scene!");
            return;
        }
        menuPanel.SetActive(false);
        Debug.Log("subscribed");
        gameStateChannel.OnMenuClicked += toggleMenu;
    }


    private void toggleMenu()
    {
        if (menuPanel != null)
        {
            bool isActive = menuPanel.activeSelf; // Current state of the panel
            Debug.Log($"MenuPanel currently {(isActive ? "active" : "inactive")}");
            isActive = !isActive;
            menuPanel.SetActive(isActive); // Toggle the panel state
            Debug.Log($"MenuPanel set to {(isActive ? "active" : "inactive")}");

            if (isActive)
            {
                gameStateChannel.StateExited(gameStateChannel.GetCurrentState());
                gameStateChannel.StateEntered(menuState);

                Debug.Log("Entered menu state.");   
            }
            else
            {
                gameStateChannel.StateEntered(menuState.nextState);
                gameStateChannel.StateExited(menuState);
                Debug.Log("Exited menu state.");
            }

            Time.timeScale = isActive ? 1 : 0; // Adjust time scale
            Debug.Log($"Time.timeScale set to {Time.timeScale}");
        }
        else
        {
            Debug.LogError("MenuPanel is null. Ensure it is assigned in the Inspector.");
        }
    }


}

