using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Chase State")]
public class ChaseStateSO : EnemyStateSO
{
    [Header("Chase Settings")]
    public float chaseSpeed = 3f;

    [Header("Transitions")]
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public EnemyStateSO attackState; 
    public EnemyStateSO patrolOrIdleState; // If player escapes, where to go?

    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool("isWalking", true);    
    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);

        if (distance > detectionRange)
        {
            enemy.StateMachine.ChangeState(enemy, patrolOrIdleState);
            return;
        }

        if (distance <= attackRange && 
            Time.time >= enemy.LastAttackTime + enemy.AttackCooldown)
        {
            enemy.StateMachine.ChangeState(enemy, attackState);
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        Vector2 direction = (enemy.player.position - enemy.transform.position).normalized;
        enemy.rb.velocity = new Vector2(direction.x * chaseSpeed, enemy.rb.velocity.y);
        enemy.UpdateFacingDirection(direction.x);
    }

    public override void OnExit(EnemyController enemy)
    {
    }
}