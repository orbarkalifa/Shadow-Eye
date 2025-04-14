using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

public abstract class Character : MonoBehaviour
{
    public Animator animator;
    public int maxHits = 5;
    public int currentHits;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private ParticleSystem damageParticleSystem;
    public int CurrentFacingDirection { get; protected set; } = 1;

    protected virtual void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        currentHits = maxHits;
    }

    public abstract void TakeDamage(int damage, float direction);

    public virtual void TakeDamage(int damage)
    {
        ActivateDamageParticles();
        currentHits -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHits}");
        animator.Play("damaged");

        if (currentHits <= 0)
        {
            OnDeath();
        }
    }
    protected virtual void OnDeath()
    {
        ActivateDeathParticles();
        Destroy(gameObject);
    }
    // Particle functions 
    void ActivateDeathParticles()
    {
        Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
    }
    void ActivateDamageParticles()
    {
        Instantiate(damageParticleSystem, transform.position, Quaternion.identity);
    }
}