using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // This method will be called when the button is clicked.
    public void LoadLevelScene(string levelName)
    {
        StartCoroutine(LoadLevelCoroutine(levelName));
        

    }

    private void OnDestroy()
    {
        beacon.uiChannel.OnChangeLevel -= LoadLevelScene;
    }
    private IEnumerator LoadLevelCoroutine(string levelName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(levelName);
        op.allowSceneActivation = false;

        // 3) Each frame, send progress and yield so the UI can update
        while (op.progress < 0.9f)
        {
            float p = Mathf.Clamp01(op.progress / 0.9f);
            beacon.uiChannel.PassLoadPercent(p);    // pass the actual float
            Debug.Log($"Loading {levelName}: {p*100:F0}%");
            yield return null;
        }

        // 4) Final “100%”
        beacon.uiChannel.PassLoadPercent(1f);
        yield return null;  // let the UI draw “full” for at least one frame

        // 5) Activate the scene
        op.allowSceneActivation = true;
        beacon.gameStateChannel.RaiseStateTransitionRequest(beacon.gameStateChannel.GetGameStateByName("In Game"));
    }
}
