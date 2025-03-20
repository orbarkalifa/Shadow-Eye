using System.Collections;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackRange;
    public int attackDamage = 1;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.3f;

    private Animator animator;
    private int comboStep;
    private float lastAttackTime;
    private readonly float comboResetTime = 1f;
    private bool isAttacking;
    private bool isOnCooldown;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void BasicAttack(Vector2 facingDirection)
    {
        if (isOnCooldown)
            return;

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
            
            attackRange.localPosition = new Vector3(facingDirection.x * 0.5f, attackRange.localPosition.y, 0f);
        }
        else if (comboStep == 1)
        {
            comboStep = 2;
            animator.CrossFadeInFixedTime("Ado_attack2", 0.05f);
        }
    }

    public void OnAttack1Complete()
    {
        doAttackHit();
    }

    public void OnAttack2Complete()
    {
        doAttackHit();
        // Only start cooldown after the second attack
        StartCoroutine(AttackCooldown());
        isAttacking = false;
        comboStep = 0;
    }
    

    private IEnumerator AttackCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    private void doAttackHit()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackRange.position,
            attackRange.GetComponent<BoxCollider2D>().size,
            0f,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemyController enemyComponent))
            {
                enemyComponent.TakeDamage(attackDamage);
            }
        }
    }
}
