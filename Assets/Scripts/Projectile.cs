using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float Speed = 10f;
    [SerializeField] private float Lifetime = 2f;
    
    private Vector2 m_Direction;

    public int Damage => Damage;
    private void Update()
    {
        transform.position += (Vector3)(m_Direction * (Speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle destructible or enemy hit
        if (collision.CompareTag("Destructible") || collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out Destructible destructible))
                destructible.TakeDamage(Damage);

            if (collision.TryGetComponent(out Character enemy))
                enemy.TakeDamage(Damage);
            
            Destroy(gameObject); // Destroy the projectile on hit
        }
    }
}