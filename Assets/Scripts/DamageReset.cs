using UnityEngine;

public class DamageReset : MonoBehaviour
{
    [SerializeField] private float defaultRecoilForce = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyRecoil()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
        if (rb != null)
        {
            rb.AddForce(direction * defaultRecoilForce, ForceMode2D.Impulse);
        }
    }
}