using UnityEngine;
using UnityEngine.Serialization;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected float m_MaxHp = 100f;
    protected float m_CurrentHp;
    public GameObject m_CurrentWeapon; // Reference to the player's active weapon
    public Transform m_WeaponHolder; // Transform where weapons are attached to the player

    
    protected virtual void Awake()
    {
        m_CurrentHp = m_MaxHp; // Initialize current health
    }

    public float CurrentHp => m_CurrentHp;

    public void TakeDamage(float i_Damage)
    {
        m_CurrentHp -= i_Damage;
        Debug.Log($"{gameObject.name} took {i_Damage} damage. HP: {m_CurrentHp}");

        if (m_CurrentHp <= 0)
        {
            OnDeath();
        }
    }

    public void Heal(float i_HealAmount)
    {
        m_CurrentHp = Mathf.Min(m_CurrentHp + i_HealAmount, m_MaxHp);
    }

    protected virtual void OnDeath()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}