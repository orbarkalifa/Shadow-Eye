using System;
using System.Collections.Generic;
using GameStateManagement;
using UnityEngine;

public class UIEyeBar : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private BeaconSO beacon;
    [SerializeField] private GameObject [] points;

    private void Awake()
    {
        beacon.uiChannel.OnChangeHealth += UpdateHUD;
    }

    private void Start()
    {
        // Create a fixed number of eyes once.
        for (int i = 0; i < maxHealth; i++)
        { 
            points[i].SetActive(true);
        }
        beacon.uiChannel.ChangeHealth(maxHealth);

    }

    private void UpdateHUD(int health)
    {
        // Update the sprite on each eye instead of recreating.
        for (int i = 0; i < points.Length; i++)
        {
            points[i].SetActive(i <= health? true : false);
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

