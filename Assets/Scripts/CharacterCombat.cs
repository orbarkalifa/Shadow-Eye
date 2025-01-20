using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject m_ProjectilePrefab;
    [SerializeField] private Transform m_FirePoint;
    [SerializeField] List<GameObject> m_WeaponPrefabs;
    
    public Transform m_AttackRange; 
    public int m_AttackDamage = 1;
    public LayerMask m_EnemyLayer;

    
    public void BasicAttack(Vector2 facingDirection)
    {
        if (m_AttackRange == null)
        {
            Debug.LogError("Attack range is not set!");
            return;
        }
        Vector3 newAttackRangePosition = new Vector3(facingDirection.x * 0.5f, m_AttackRange.localPosition.y, 0f);
        m_AttackRange.localPosition = newAttackRangePosition;

        
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            m_AttackRange.position,
            m_AttackRange.GetComponent<BoxCollider2D>().size,
            0f,
            m_EnemyLayer
        );
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<EnemyController>(out EnemyController enemyComponent))
            {
                enemyComponent.TakeDamage(m_AttackDamage);
            }
        }
    }
    
}
