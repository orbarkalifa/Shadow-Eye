using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField]
    private GameObject menuPanel;
    private GameStateChannel m_GameStateChannel;
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
        
        m_GameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;
        if (m_GameStateChannel == null)
        {
            Debug.LogError("gameStateChannel not found in the scene!");
            return;
        }
        menuPanel.SetActive(false);
        Debug.Log("subscribed");
        m_GameStateChannel.OnMenuClicked += OnToggleMenu;
    }


    public void OnToggleMenu()
    {
        // You could check here if the current state allows for the menu
        GameState current = m_GameStateChannel.GetCurrentGameState();
        if(!current || !current.stateSO.canMenu)
        {
            return;
        }
        bool isActive = menuPanel.activeSelf;
        menuPanel.SetActive(!isActive);

        // Pause/unpause
        Time.timeScale = menuPanel.activeSelf ? 0f : 1f;
    }


}

