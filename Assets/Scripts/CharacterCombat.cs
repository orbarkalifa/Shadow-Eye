using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform AttackRange; 
    public int AttackDamage = 1;
    public LayerMask EnemyLayer;

    
    public void BasicAttack(Vector2 facingDirection)
    {
        if (AttackRange == null)
        {
            Debug.LogError("Attack range is not set!");
            return;
        }
        Vector3 newAttackRangePosition = new Vector3(facingDirection.x * 0.5f, AttackRange.localPosition.y, 0f);
        AttackRange.localPosition = newAttackRangePosition;
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            AttackRange.position,
            AttackRange.GetComponent<BoxCollider2D>().size,
            0f,
            EnemyLayer
        );
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<EnemyController>(out EnemyController enemyComponent))
            {
                enemyComponent.TakeDamage(AttackDamage);
            }
        }
    }
    
}
