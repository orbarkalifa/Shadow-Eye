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
    private float comboResetTime = 1f; 
    private bool isAttacking = false; 

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

        if (!isAttacking)
        {
            comboStep = 1;
            isAttacking = true; 
            animator.CrossFadeInFixedTime("Ado_attack1", 0.05f);

            Vector3 newAttackRangePos = new Vector3(facingDirection.x * 0.5f, AttackRange.localPosition.y, 0f);
            AttackRange.localPosition = newAttackRangePos;
        }
        else if (comboStep == 1)
        {
            comboStep = 2;
        }
    }
    public void OnAttack1Complete()
    {
        DoAttackHit();
        if (comboStep == 2)
        {
           animator.CrossFadeInFixedTime("Ado_attack2", 0.05f);
        }
        else
        {
            isAttacking = false;
            comboStep = 0;
        }
    }
    public void OnAttack2Complete()
    {
        DoAttackHit();
        isAttacking = false;
        comboStep = 0;
    }

    
    public void DoAttackHit()
    {
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
