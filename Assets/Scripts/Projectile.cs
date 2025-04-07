using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle destructible or enemy hit
        if (collision.CompareTag("Destructible") || collision.CompareTag("Enemy"))
        {
            float recoilDirection = transform.localScale.x > 0 ? -1 : 1;
            if (collision.TryGetComponent(out Destructible destructible))
                destructible.TakeDamage(damage);

            if (collision.TryGetComponent(out Character enemy))
                enemy.TakeDamage(damage,recoilDirection);
            
            Destroy(gameObject); // Destroy the projectile on hit
        }
    }
}