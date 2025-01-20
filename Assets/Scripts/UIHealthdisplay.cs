using System.Collections;
using System.Collections.Generic;
using Scriptable.Scripts;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIHealthdisplay : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    // Reference to the Text component
    private HealthChannelSo healthChannel;
    
    void Awake()
    {
        healthChannel = FindObjectOfType<Beacon>().healthChannel;
        if(healthChannel != null)
        {
            Debug.Log("Found healtChannel in UIHealthdisplay");
        }
        healthChannel.OnChangeHealth += updateText;
        healthText = GetComponent<TextMeshProUGUI>();
    }
    void updateText(int health)
    {
        Debug.Log($"Health changed to {health}");
        healthText.text = $"{health}";
    }
}
