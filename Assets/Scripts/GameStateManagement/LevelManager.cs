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
        
        var scene = SceneManager.LoadSceneAsync(levelName);
        scene.allowSceneActivation = false;
        beacon.uiChannel.PassLoadPercent(0);
        do
        {
            
            beacon.uiChannel.PassLoadPercent(Mathf.Clamp01(scene.progress / 0.9f));
            Debug.Log($"loading level {levelName} progress: {Mathf.Clamp01(scene.progress / 0.9f)}");
            
        }
        while(scene.progress < 0.9f);
        scene.allowSceneActivation = true;
        beacon.gameStateChannel.RaiseStateTransitionRequest(beacon.gameStateChannel.GetGameStateByName("In Game"));
    }

    private void OnDestroy()
    {
        beacon.uiChannel.OnChangeLevel -= LoadLevelScene;
    }
}
