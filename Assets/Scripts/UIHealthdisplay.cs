using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable.Scripts;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIHealthdisplay : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    
    
    private HealthChannelSo healthChannel;
    private int maxHealth;
    private int currentHealth;
    [SerializeField]private Image[] eyes;
    [SerializeField]private Sprite eyeSprite;
    [SerializeField]private Sprite deadeyeSprite;
    
    void Awake()
    {
        healthChannel = FindObjectOfType<Beacon>().healthChannel;
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

}
