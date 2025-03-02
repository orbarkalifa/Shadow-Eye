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
    
    void Awake()
    {
        healthChannel = FindObjectOfType<Beacon>().healthChannel;
        healthChannel.OnChangeHealth += updateText;
        healthText = GetComponent<TextMeshProUGUI>();
    }
    void updateText(int health)
    {
        if(healthText != null)
        {
            healthText.text = $"{health}";
        }
    }
}
