using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float m_Health = 50f;
    [SerializeField] private bool m_DestroyOnHit = false;

    public void TakeDamage(float damage)
    {
        if (m_DestroyOnHit)
        {
            Destroy(gameObject);
            return;
        }

        m_Health -= damage;
        if (m_Health <= 0)
            Destroy(gameObject);
    }
}