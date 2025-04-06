using System.Collections;
using UnityEngine;

public class DamageReset : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float defaultRecoilForce = 5f;
    
    [Header("Flashing Settings")]
    [SerializeField] private float flashDuration = 1.0f;
    [SerializeField] private float flashInterval = 0.1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    
    public void AppleKnockback(Transform damageSource = null)
    {
        Vector2 knockbackDirection;
        if (damageSource != null)
        {
            knockbackDirection = ((Vector2)transform.position - (Vector2)damageSource.position).normalized;
        }
        else
        {
            knockbackDirection = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(knockbackDirection * defaultRecoilForce, ForceMode2D.Impulse);
        }

        StartCoroutine(FlashSprite());
    }

    private IEnumerator FlashSprite()
    {
        if (sr == null)
            yield break;

        float timer = 0f;
        while (timer < flashDuration)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
            yield return new WaitForSeconds(flashInterval);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval * 2;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }
}
