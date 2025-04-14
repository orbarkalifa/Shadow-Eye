using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameStateManagement
{
    public class UIManager : MonoBehaviour
    {
        
        public static UIManager Instance { get; private set; }
        
        [Header("UI Panels")]
        [SerializeField] private GameObject startMenuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject inGameHUDPanel;
        [SerializeField] private GameObject winGameHUDPanel;
        private BeaconSO beacon;
        private void Awake()
        {
            if (Instance != null && Instance != this || FindObjectsByType<UIManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void HideAllPanels()
        {
            Debug.Log("Hiding all ui panels");
            if(startMenuPanel) startMenuPanel.SetActive(false);
            if(pauseMenuPanel) pauseMenuPanel.SetActive(false);
            if(gameOverPanel) gameOverPanel.SetActive(false);
            if(inGameHUDPanel) inGameHUDPanel.SetActive(false);
            if(winGameHUDPanel) winGameHUDPanel.SetActive(false);
        }

        private void SetSelectedUI(GameObject selectable)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectable);
        }


        public void ShowStartMenuPanel()
        {
            HideAllPanels();
            if (startMenuPanel != null)
            {
                startMenuPanel.SetActive(true);
                var defaultButton = startMenuPanel.GetComponentInChildren<Button>();
                if (defaultButton != null)
                    SetSelectedUI(defaultButton.gameObject);

                Debug.Log("UIManager: Start Menu Panel activated. Active state: " + startMenuPanel.activeSelf);
            }
            else
            {
                Debug.LogError("UIManager: Start Menu Panel is not assigned!");
            }
        }

        public void ShowPauseMenuPanel()
        {
            HideAllPanels();
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
                var defaultButton = pauseMenuPanel.GetComponentInChildren<Button>();
                if (defaultButton != null)
                    SetSelectedUI(defaultButton.gameObject);
                Debug.Log("UIManager: Pause Menu Panel activated.");
            }
            else
            {
                Debug.LogError("UIManager: Pause Menu Panel is not assigned!");
            }
        }

        public void ShowGameOverPanel()
        {
            HideAllPanels();
            if(gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                var defaultButton = gameOverPanel.GetComponentInChildren<Button>();
                if (defaultButton != null)
                    SetSelectedUI(defaultButton.gameObject);
                Debug.Log("UIManager: Game Over Panel activated.");
            }
            else
            {
                Debug.LogError("UIManager: Game Over Panel is not assigned!");
            }
        }

        public void ShowInGameHUDPanel()
        {
            HideAllPanels();
            if (inGameHUDPanel != null)
            {
                inGameHUDPanel.SetActive(true);
                Debug.Log("UIManager: In-Game HUD Panel activated.");
            }
            else
            {
                Debug.LogError("UIManager: In-Game HUD Panel is not assigned!");
            }
        }

        public void ShowWinGameHUDPanel()
        {
            HideAllPanels();
            if(winGameHUDPanel != null)
            {
                winGameHUDPanel.SetActive(true);
                Debug.Log("UIManager: Win HUD Panel activated");
            }
            else
            {
                Debug.LogError("UIManager: Win HUD Panel is not assigned!");
            }
        }
    }
}
