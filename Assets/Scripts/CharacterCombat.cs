using System;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform AttackRange; 
    public int AttackDamage = 1;
    public LayerMask EnemyLayer;
    
    private Animator animator;
    private int comboStep = 0;
    private float lastAttackTime;
    private float comboResetTime = 1f; // Reset combo if inactive too long
    private bool isAttacking = false; // Prevent attack spamming

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void BasicAttack(Vector2 facingDirection)
    {
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
            isAttacking = false;
        }

        lastAttackTime = Time.time;

        //    If we are NOT currently attacking or finishing an attack,
        //    start Attack1
        if (!isAttacking)
        {
            comboStep = 1;      // Attack1
            isAttacking = true; // We are busy
            animator.CrossFadeInFixedTime("Ado_attack1", 0.05f);

            // Position the AttackRange based on facing direction
            Vector3 newAttackRangePos = new Vector3(facingDirection.x * 0.5f, AttackRange.localPosition.y, 0f);
            AttackRange.localPosition = newAttackRangePos;
        }
        //    If we ARE attacking and we’re on Attack1,
        //    queue the second attack
        else if (comboStep == 1)
        {
            // The second attack is now queued; we do NOT immediately play Attack2
            // Instead, we let the end of Attack1 (Animation Event) trigger Attack2.
            comboStep = 2;
        }
    }
    public void OnAttack1Complete()
    {
        DoAttackHit();
        // If the player queued the 2nd attack by pressing attack again
        if (comboStep == 2)
        {
           animator.CrossFadeInFixedTime("Ado_attack2", 0.05f);
        }
        else
        {
            // If no second attack is queued, end the combo
            isAttacking = false;
            comboStep = 0;
        }
    }
    public void OnAttack2Complete()
    {
        DoAttackHit();
        // Attack2 is finished. End the combo.
        isAttacking = false;
        comboStep = 0;
    }

    
    public void DoAttackHit()
    {
        // Perform the actual overlap detection here.
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
