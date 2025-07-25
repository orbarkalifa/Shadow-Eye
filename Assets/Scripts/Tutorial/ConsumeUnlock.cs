using System.Collections;
using System.Collections.Generic;
using GameStateManagement;
using UnityEngine;

public class ConsumeUnlock : MonoBehaviour
{
    [SerializeField] private string tutorialMessage = "";
    [SerializeField] private float duration = 3f;
    [SerializeField] private TutorialPanelController tutorialPanel;
    [SerializeField] private PlayerChannel playerChannel;
    private bool triggered;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        // Raise an event on the channel instead of calling the player directly
        playerChannel.UnlockConsume();

        if (GSManager.Instance.tutorialsEnabled)
        {
            tutorialPanel.ShowMessage(tutorialMessage, duration);
            triggered = true;
        }
    }

}

    