
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Character : MonoBehaviour
{
    public int maxHits = 5;
    public int currentHits;
    
    protected virtual void Awake()
    {
        currentHits = maxHits;
    }
    
    public virtual void TakeDamage(int damage)
    {
        currentHits -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHits}");
        if (currentHits <= 0)
        {
            OnDeath();
        }
    }
    protected virtual void OnDeath()
    {
        Destroy(gameObject); 
    }
}