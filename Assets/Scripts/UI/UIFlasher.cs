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
            enabled = false; 
            return;
        }
        baseColor = imageToFlash.color;
    }

    void Update()
    {
        if (imageToFlash == null) return;
        float alphaRange = maxAlpha - minAlpha;
        float pingPongValue = Mathf.PingPong(Time.unscaledTime * flashSpeed, 1.0f);
        float targetAlpha = minAlpha + pingPongValue * alphaRange;

        imageToFlash.color = new Color(baseColor.r, baseColor.g, baseColor.b, targetAlpha);
    }

    void OnDisable()
    {
        if (imageToFlash != null)
        {
            imageToFlash.color = new Color(baseColor.r, baseColor.g, baseColor.b, maxAlpha);
        }
    }

    void OnEnable()
    {
        if (imageToFlash != null) {
             baseColor = imageToFlash.color;
        }
    }
}