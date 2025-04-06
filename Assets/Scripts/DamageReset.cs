using System.Collections;
using UnityEngine;

public class DamageReset : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float defaultRecoilForce = 5f;
    
    [Header("Flashing Settings")]
    [SerializeField] private float flashDuration = 1.0f;  // How long the flash effect lasts
    [SerializeField] private float flashInterval = 0.1f;  // Time between alpha toggles

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Applies knockback and starts a flashing effect.
    /// If a damage source is provided, knockback is calculated away from that source.
    /// Otherwise, it defaults to using the local scale to determine direction.
    /// </summary>
    /// <param name="damageSource">Optional Transform of the damage source (enemy)</param>
    public void ApplyRecoil(Transform damageSource = null)
    {
        Vector2 knockbackDirection;
        if (damageSource != null)
        {
            // Calculate knockback as a normalized vector from the enemy to the player.
            knockbackDirection = ((Vector2)transform.position - (Vector2)damageSource.position).normalized;
        }
        else
        {
            // Fallback: determine direction based on the local scale.
            knockbackDirection = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
        }

        if (rb != null)
        {
            // Zero out current velocity for consistent knockback.
            rb.velocity = Vector2.zero;
            rb.AddForce(knockbackDirection * defaultRecoilForce, ForceMode2D.Impulse);
        }

        // Start flashing effect to indicate invincibility.
        StartCoroutine(FlashSprite());
    }

    /// <summary>
    /// Coroutine to flash the sprite by toggling its alpha.
    /// </summary>
    private IEnumerator FlashSprite()
    {
        if (sr == null)
            yield break;

        float timer = 0f;
        while (timer < flashDuration)
        {
            // Set to half-transparent.
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
            yield return new WaitForSeconds(flashInterval);
            // Restore full opacity.
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval * 2;
        }
        // Ensure the sprite's alpha is reset to fully opaque.
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }
}
