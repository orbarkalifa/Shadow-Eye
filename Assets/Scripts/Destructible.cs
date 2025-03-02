using UnityEngine;
using UnityEngine.Serialization;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float Health = 50f;
    [SerializeField] private bool DestroyOnHit = false;

    public void TakeDamage(float damage)
    {
        if (DestroyOnHit)
        {
            Destroy(gameObject);
            return;
        }
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
    }
}