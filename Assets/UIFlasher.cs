using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIFlasherPingPong : MonoBehaviour
{
    public float flashSpeed = 1.0f;
    public float minAlpha = 0.0f;
    public float maxAlpha = 1.0f;

    private Image imageToFlash;
    private Color baseColor;

    void Start()
    {
        imageToFlash = GetComponent<Image>();
        if (imageToFlash == null)
        {
            Debug.LogError("UIFlasherPingPong requires an Image component.", this);
            enabled = false; // Disable script if no image found
            return;
        }
        baseColor = imageToFlash.color;
        // Keep the original color's RGB, we only manipulate alpha
    }

    void Update()
    {
        if (imageToFlash == null) return; // Safety check

        float alphaRange = maxAlpha - minAlpha;

        // --- CHANGE IS HERE ---
        // Use Time.unscaledTime instead of Time.time
        float pingPongValue = Mathf.PingPong(Time.unscaledTime * flashSpeed, 1.0f);
        // ----------------------

        float targetAlpha = minAlpha + pingPongValue * alphaRange;

        // Use the baseColor's RGB and the calculated targetAlpha
        imageToFlash.color = new Color(baseColor.r, baseColor.g, baseColor.b, targetAlpha);
    }

    // Optional: Reset alpha when disabled (consider if you want maxAlpha or the original alpha)
    void OnDisable()
    {
        // Resetting might be desirable if the object is temporarily disabled
        // and you want it to reappear fully opaque when re-enabled,
        // rather than potentially reappearing partially transparent.
        if (imageToFlash != null)
        {
            // Decide what state is best on disable. Maybe the original alpha?
            // Or maxAlpha as before? Let's stick with maxAlpha for now.
            imageToFlash.color = new Color(baseColor.r, baseColor.g, baseColor.b, maxAlpha);
        }
    }

     // Optional: Ensure baseColor is correct if the component is enabled after Start
    void OnEnable()
    {
        if (imageToFlash != null) {
             // Re-capture base color in case it was changed while disabled
             // Although unlikely if only alpha is being changed by this script.
             baseColor = imageToFlash.color;
        }
        // No need to restart anything, Update will handle it.
    }
}