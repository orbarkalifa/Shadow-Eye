using UnityEngine;

public class WallGrabUnlock : MonoBehaviour
{
    [SerializeField] private string tutorialMessage = "Wall Grab Unlocked!\nHold toward wall + Jump to cling!";
    [SerializeField] private float duration = 3f;
    [SerializeField] private TutorialPanelController tutorialPanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!GameStateManagement.GSManager.Instance.tutorialsEnabled) return;

        var player = other.GetComponent<MainCharacter>();
        if (player != null)
        {
            player.UnlockWallGrabAbility();
            tutorialPanel.ShowMessage(tutorialMessage, duration);
            Destroy(gameObject);
        }
    }
}