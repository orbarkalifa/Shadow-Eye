using UnityEngine;
using UnityEngine.Serialization;

public class Destructible : MonoBehaviour
{
    [SerializeField]  ParticleSystem hitParticles;
    [SerializeField] ParticleSystem destoyParticles;
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
        if(Health <= 0)
        {
            Instantiate(destoyParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}