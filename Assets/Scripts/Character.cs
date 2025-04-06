
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Character : MonoBehaviour
{
    public Animator animator;
    public int maxHits = 5;
    public int currentHits;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private ParticleSystem damageParticleSystem;
    [SerializeField] private DamageReset damageReset;
    public int CurrentFacingDirection { get; set; } = 1;

    protected virtual void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
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

        damageReset.AppleKnockback();
    }
    
    protected virtual void OnDeath()
    {
        ActivateDeathParticles();
        Destroy(gameObject); 
    }
    //particles functions 
    void ActivateDeathParticles()
    {
        Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
    }
    void ActivateDamageParticles()
    {
        Instantiate(damageParticleSystem, transform.position, Quaternion.identity);
    }
}