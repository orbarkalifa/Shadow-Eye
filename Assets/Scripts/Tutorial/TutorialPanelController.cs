using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialPanelController : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float fadeDuration = 0.25f;

    private Coroutine currentRoutine;
    
    public void ShowMessage(string message, float displayDuration)
    {
        messageText.text = message;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        panel.SetActive(true);
        currentRoutine = StartCoroutine(ShowAndHideRoutine(displayDuration));
    }

    private IEnumerator ShowAndHideRoutine(float duration)
    {
        yield return FadeCanvas(0f, 1f); // Fade in

        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            yield return FadeCanvas(1f, 0f); // Fade out
            panel.SetActive(false);
        }

        currentRoutine = null;
    }

    private IEnumerator FadeCanvas(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void HideImmediately()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        canvasGroup.alpha = 0f;
        panel.SetActive(false);
    }
}