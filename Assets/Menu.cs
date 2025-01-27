using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField]
    private GameObject menuPanel;
    private GameStateChannel gameStateChannel;

    private void Awake()
    {
        gameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;
        Debug.Log("subscribed");
 
        gameStateChannel.OnMenuClicked += ToggleMenu;
    }

    private void OnDisable()
    {
        gameStateChannel.OnMenuClicked -= ToggleMenu;
    }

    private void ToggleMenu()
    {
        Debug.Log("Etered toggling menu");
        if(menuPanel != null)
        {
            bool isActive = menuPanel.activeSelf;
            menuPanel.SetActive(!isActive);
            Time.timeScale = isActive ? 1 : 0;
        }
    }
}

