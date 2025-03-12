using System.Collections.Generic;
using Scriptable.Scripts;
using UnityEngine;

public class UIEyeBar : MonoBehaviour
{
    private List<UIEyeScript> eyes = new List<UIEyeScript>();
    [SerializeField] private GameObject EyePrefab;
    [SerializeField] private int maxHealth = 5; // or retrieve from player data
    private HealthChannelSo healthChannel;

    private void Awake()
    {
        healthChannel = FindObjectOfType<Beacon>().healthChannel;
        healthChannel.OnChangeHealth += UpdateHUD;
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
        if (healthChannel != null)
        {
            healthChannel.OnChangeHealth -= UpdateHUD;
        }
    }
}

