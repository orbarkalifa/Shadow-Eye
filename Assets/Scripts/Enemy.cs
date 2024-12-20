using UnityEngine;

public class Enemy : Character
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            if (projectile != null)
                TakeDamage(projectile.Damage);
        }
    }

    protected override void OnDeath()
    {
        Debug.Log($"{gameObject.name} (Enemy) has been defeated!");
        if (m_CurrentWeapon != null)
        {
            Debug.Log($"Dropping weapon: {m_CurrentWeapon.name}");
            Instantiate(m_CurrentWeapon, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("m_CurrentWeapon is null, no weapon to drop.");
        }
        base.OnDeath();
    }
    
}