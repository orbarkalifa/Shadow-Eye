
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int MaxHits = 5;
    public int CurrentHits;
    
    protected virtual void Awake()
    {
        CurrentHits = MaxHits;
    }
    
    public virtual void TakeDamage(int damage)
    {
        CurrentHits -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {CurrentHits}");
        if (CurrentHits <= 0)
        {
            OnDeath();
        }
    }
    protected virtual void OnDeath()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject); 
    }
}