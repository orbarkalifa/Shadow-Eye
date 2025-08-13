using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; protected set; }
    public SpriteRenderer sr { get; private set; }

    
    public int maxHits = 5;
    public int currentHits;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private ParticleSystem damageParticleSystem;
    [SerializeField] protected BeaconSO beacon;

    public bool IsInvincible { get; protected set; } // New property for invincibility

    public int CurrentFacingDirection { get; protected set; } = 1;

    protected virtual void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        currentHits = maxHits;
    }

    public abstract void TakeDamage(int damage, Vector2 source);

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
    protected Vector2 GetRecoilDirection(Vector2 target)
    {
        return ((Vector2)transform.position - target).normalized;
    }
}