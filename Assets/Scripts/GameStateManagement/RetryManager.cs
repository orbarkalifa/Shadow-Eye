using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace GameStateManagement
{
    public class RetryManager : MonoBehaviour
    {
        public static RetryManager Instance { get; private set; }

        public BeaconSO beacon;
        
        [SerializeField] private GameStateSO inGameState;
        
        private PlayerSaveData retryData;
        private bool isRetrying;
        [SerializeField] private string defaultStartLevelName = "IntroLevel";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var prefab = Resources.Load<RetryManager>("RetryManager");
            if (prefab)
            {
                Debug.Log("RetryManager loaded from resource");
                Instance = Instantiate(prefab);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            beacon.playerDeathChannel.OnPlayerDied += HandlePlayerDeath;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoadedToClearData;
        }

        private void OnDisable()
        {
            beacon.playerDeathChannel.OnPlayerDied -= HandlePlayerDeath;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded -= OnSceneLoadedToClearData;
        }

        private void HandlePlayerDeath(PlayerSaveData data)
        {
            Debug.Log("<color=orange>EVENT RECEIVED:</color> RetryManager heard player death.");
            retryData = data;
            SaveSystem.SaveOnDeath(retryData);
        }

        public void AttemptRetry()
        {
            Debug.Log("<color=cyan>RETRY ATTEMPTED:</color> Try Again button was clicked.");
            retryData = SaveSystem.LoadForRetry();
            PlayerSaveData dataFromFile = SaveSystem.LoadForRetry();
            if (dataFromFile == null || string.IsNullOrEmpty(dataFromFile.sceneToLoad))
            {
                Debug.LogWarning("RETRY FAILED: No valid retry data found. Performing a clean restart of the default level.");
                beacon.uiChannel.ChangeLevel(defaultStartLevelName, inGameState);
                return;
            }
            isRetrying = true;
            beacon.uiChannel.ChangeLevel(retryData.sceneToLoad, inGameState);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!isRetrying) return;
            Debug.Log("<color=green>SCENE LOADED for Retry:</color> Now attempting to apply player data.");
            Player.PlayerController player = FindObjectOfType<Player.PlayerController>();
            if (player)
            {
                player.ApplyRetryData(retryData);
            }
            isRetrying = false;
            retryData = null; 
            Debug.Log("RetryManager: Retry process complete. Data has been cleared.");
        }
        
        private void OnSceneLoadedToClearData(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "startScene")
            {
                isRetrying = false;
                retryData = null;
                string path = Path.Combine(Application.persistentDataPath, "retryData.json");
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                        Debug.Log("<color=red>RETRY FILE DELETED:</color> Physical retry file has been removed.");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Failed to delete retry file: " + e.Message);
                    }
                }
                else
                {
                    Debug.Log("RetryManager: Main Menu loaded. No retry file to delete, which is normal.");
                }
            }
        }
    }
}