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
        gameStateChannel.OnMenuClicked += OnToggleMenu;
    }


    public void OnToggleMenu()
    {
        // You could check here if the current state allows for the menu
        var current = gameStateChannel.GetCurrentGameState();
        if (current && current.stateSO.canMenu)
        {
            bool isActive = menuPanel.activeSelf;
            menuPanel.SetActive(!isActive);

            // Pause/unpause
            Time.timeScale = menuPanel.activeSelf ? 0f : 1f;
        }
    }


}

