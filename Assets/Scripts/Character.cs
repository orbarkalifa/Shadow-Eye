
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Character : MonoBehaviour
{
    [FormerlySerializedAs("m_MaxHp")]
    [SerializeField] protected int MaxHits = 5;
    public int CurrentHits;
    

    protected virtual void Awake()
    {
        
        CurrentHits = MaxHits;

    }
    

    public void TakeDamage(int damage)
    {
        CurrentHits -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {CurrentHits}");

        if (CurrentHits <= 0)
        {
            OnDeath();
        }
    }

    public void Heal()
    {
        CurrentHits = Mathf.Min(CurrentHits + 1, MaxHits);
    }

    protected virtual void OnDeath()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject); 
    }
    protected virtual int GetCurrentHits(){return CurrentHits;}
}