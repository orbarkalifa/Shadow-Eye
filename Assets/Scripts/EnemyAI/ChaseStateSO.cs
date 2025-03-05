using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Chase State")]
public class ChaseStateSO : EnemyStateSO
{
    [Header("Chase Settings")]
    public float chaseSpeed = 3f;

    [Header("Transitions")]
    [Tooltip("Stop chasing if player goes beyond this distance.")]
    public float detectionRange = 5f;

    [Tooltip("If the player is within this range, go to 'attackState'.")]
    public float attackRange = 1.5f;

    public EnemyStateSO attackState; 
    public EnemyStateSO patrolOrIdleState; // If player escapes, where to go?

    public override void OnEnter(EnemyController enemy)
    {
        Debug.Log("STATE: Chase -> OnEnter");
    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);

        // If out of detection range, return to patrol/idle
        if (distance > detectionRange)
        {
            enemy.StateMachine.ChangeState(enemy, patrolOrIdleState);
            return;
        }

        // If close enough to attack AND cooldown is ready
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
        Debug.Log("STATE: Chase -> OnExit");
    }
}