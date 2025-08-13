using UnityEngine;
using GameStateManagement;

public class WallGrabUnlock : MonoBehaviour
{
    [SerializeField] private string tutorialMessage = "Wall Grab Unlocked!\nHold toward wall + Jump to cling!";
    [SerializeField] private float duration = 3f;
    [SerializeField] private TutorialPanelController tutorialPanel;
    [SerializeField] private PlayerChannel playerChannel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Raise an event on the channel instead of calling the player directly
        playerChannel.UnlockWallGrab();

        if (GSManager.Instance.tutorialsEnabled)
        {
            tutorialPanel.ShowMessage(tutorialMessage, duration);
        }
        GSManager.Instance.DisableTutorials();
        Destroy(gameObject);
    }
}