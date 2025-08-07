using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    public class LevelManager : MonoBehaviour
    {
        public BeaconSO beacon;
    
        private void Awake()
        {
            if (FindObjectsByType<LevelManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            if(beacon == null)
                Debug.Log("no beacon");
            beacon.uiChannel.OnChangeLevel += LoadLevelScene;
        }

        public void LoadLevelScene(string levelName, GameStateSO state)
        {
            StartCoroutine(LoadLevelCoroutine(levelName,state));
        }

        private void OnDestroy()
        {
            beacon.uiChannel.OnChangeLevel -= LoadLevelScene;
        }
        private IEnumerator LoadLevelCoroutine(string levelName, GameStateSO stateToActivate)
        {
            beacon.uiChannel.PassLoadPercent(0f);
            yield return null; // Wait a frame for UI to update

            AsyncOperation op = SceneManager.LoadSceneAsync(levelName);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                beacon.uiChannel.PassLoadPercent(progress);
                yield return null;
            }

            beacon.uiChannel.PassLoadPercent(1f);
            op.allowSceneActivation = true;

            while (!op.isDone)
            {
                yield return null;
            }

            if (stateToActivate != null)
            {
                beacon.gameStateChannel.RaiseStateTransitionRequest(stateToActivate);
            }
            else
            {
                Debug.LogWarning($"Level {levelName} loaded without a target GameStateSO to activate.");
            }
        }
    }
}
