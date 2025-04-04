
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Character : MonoBehaviour
{
    public Animator animator;
    public int maxHits = 5;
    public int currentHits;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private ParticleSystem damageParticleSystem;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        currentHits = maxHits;
    }
    
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
    //particles functions 
    void ActivateDeathParticles()
    {
        deathParticleSystem = Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
        deathParticleSystem.Play();
    }
    void ActivateDamageParticles()
    {
        damageParticleSystem = Instantiate(damageParticleSystem, transform.position, Quaternion.identity);
        damageParticleSystem.Play();
    }
}