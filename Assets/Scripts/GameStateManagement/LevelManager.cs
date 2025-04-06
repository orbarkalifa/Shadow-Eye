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
        do
        {
            Debug.Log($"loading level {levelName} progress: {scene.progress}");
        }
        while(scene.progress < 0.9f);
        scene.allowSceneActivation = true;
    }

    private void OnDestroy()
    {
        beacon.uiChannel.OnChangeLevel -= LoadLevelScene;
    }
}
