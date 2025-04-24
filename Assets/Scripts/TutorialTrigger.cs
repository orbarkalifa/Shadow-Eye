using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private string tutorialMessage;
    [SerializeField] private float duration = 3f;
    [SerializeField] private TutorialPanelController tutorialPanel;
    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        if (!GameStateManagement.GSManager.Instance.tutorialsEnabled) return;

        tutorialPanel.ShowMessage(tutorialMessage, duration);
        triggered = true;
    }
}