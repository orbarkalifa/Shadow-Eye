using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class ConsumeUnlock : MonoBehaviour
{
    [SerializeField] private string tutorialMessage = "Wall Grab Unlocked!\nHold toward wall + Jump to cling!";
    [SerializeField] private float duration = 3f;
    [SerializeField] private TutorialPanelController tutorialPanel;
    private bool triggered;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered||!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.UnlockConsumeAbility();
            if (GameStateManagement.GSManager.Instance.tutorialsEnabled)
            {
                tutorialPanel.ShowMessage(tutorialMessage, duration);
                triggered = true;
            }
        }
    }
}

    