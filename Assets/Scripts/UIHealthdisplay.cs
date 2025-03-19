using System;
using System.Collections;
using System.Collections.Generic;
using GameStateManagement;
using Scriptable.Scripts;
using UnityEngine;
using TMPro;

public class UIHealthdisplay : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    
    
    private HealthChannelSo healthChannel;
    private int maxHealth;
    private int currentHealth;
    [SerializeField]private Sprite eyeSprite;
    [SerializeField]private Sprite deadeyeSprite;
    private GSManager gsManager;
    
    void Awake()
    {
        gsManager = FindObjectOfType<GSManager>();
        healthChannel = gsManager.beacon.healthChannel;
        healthChannel.OnChangeHealth += updateText;
        healthText = GetComponent<TextMeshProUGUI>();
    }
    

    void updateText(int health)
    {
        currentHealth = health;
        if(healthText != null)
        {
            healthText.text = $"{health}";
        }
    }

    private void OnDestroy()
    {
        if (healthChannel != null)
        {
            healthChannel.OnChangeHealth -= updateText;
        }
    }

}
