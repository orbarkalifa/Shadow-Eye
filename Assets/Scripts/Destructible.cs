using Suits;
using UnityEngine;
using UnityEngine.Serialization;

public class Destructible : MonoBehaviour
{
    [SerializeField]  ParticleSystem hitParticles;
    [SerializeField] ParticleSystem destoyParticles;
    [SerializeField] protected float health = 50f;

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Instantiate(destoyParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}