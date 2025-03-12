using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Patrol State")]
public class PatrolStateSO : EnemyStateSO
{
    [Header("Movement")]
    public float patrolSpeed = 2f;

    [Header("Transitions")]
    public float detectionRange = 10f;
    public EnemyStateSO chaseState;

    public override void OnEnter(EnemyController enemy)
    {
        // Possibly set a patrol animation
    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (distance <= detectionRange)
        {
            // Switch to chase
            enemy.StateMachine.ChangeState(enemy, chaseState);
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        // If no patrol points, do nothing or go idle
        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0) return;

        Transform targetPoint = enemy.patrolPoints[enemy.currentPatrolIndex];
        Vector2 direction = (targetPoint.position - enemy.transform.position).normalized;

        enemy.rb.velocity = new Vector2(direction.x * patrolSpeed, enemy.rb.velocity.y);
        enemy.UpdateFacingDirection(direction.x);

        float distanceToPoint = Vector2.Distance(enemy.transform.position, targetPoint.position);
        if (distanceToPoint <= 1f)
        {
            enemy.currentPatrolIndex++;
            if (enemy.currentPatrolIndex >= enemy.patrolPoints.Length)
            {
                enemy.currentPatrolIndex = 0;
            }
        }
    }

    public override void OnExit(EnemyController enemy)
    {
    }
}