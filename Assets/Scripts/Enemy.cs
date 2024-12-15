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
        base.OnDeath();
    }
}