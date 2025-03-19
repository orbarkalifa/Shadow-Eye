using System;
using System.Collections.Generic;
using GameStateManagement;
using UnityEngine;

public class UIEyeBar : MonoBehaviour
{
    private List<UIEyeScript> eyes = new List<UIEyeScript>();
    [SerializeField] private GameObject EyePrefab;
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private BeaconSO beacon;

    private void Awake()
    {
        beacon.uiChannel.OnChangeHealth += UpdateHUD;
    }

    private void Start()
    {
        // Create a fixed number of eyes once.
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject newEye = Instantiate(EyePrefab, transform);
            UIEyeScript eyeScript = newEye.GetComponent<UIEyeScript>();
            eyes.Add(eyeScript);
        }
        beacon.uiChannel.ChangeHealth(maxHealth);

    }

    private void UpdateHUD(int health)
    {
        // Update the sprite on each eye instead of recreating.
        for (int i = 0; i < eyes.Count; i++)
        {
            eyes[i].setImage(i < health);
        }
    }

    private void OnDestroy()
    {
        if (beacon != null)
        {
            beacon.uiChannel.OnChangeHealth -= UpdateHUD;
        }
    }
}

