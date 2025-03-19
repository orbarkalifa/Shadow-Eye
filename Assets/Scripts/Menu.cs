/*
using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField]
    private GameObject menuPanel;
    private GameStateChannel gameStateChannel;

    private void Awake()
    {
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
        gameStateChannel.OnMenuClicked += OnToggleMenu;
    }


    public void OnToggleMenu()
    {
        // You could check here if the current state allows for the menu
        GameState current = gameStateChannel.GetCurrentGameState();
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
*/

