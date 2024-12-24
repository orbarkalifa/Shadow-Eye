using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_Lifetime = 2f;
    [SerializeField] private float m_Damage = 10f;
    
    private Vector2 m_Direction;

    public float Damage => m_Damage;

    public void Initialize(Vector2 direction)
    {
        m_Direction = direction.normalized;
        Destroy(gameObject, m_Lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(m_Direction * (m_Speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle destructible or enemy hit
        if (collision.CompareTag("Destructible") || collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out Destructible destructible))
                destructible.TakeDamage(m_Damage);

            if (collision.TryGetComponent(out Character enemy))
                enemy.TakeDamage(m_Damage);

            Destroy(gameObject); // Destroy the projectile on hit
        }
    }
}