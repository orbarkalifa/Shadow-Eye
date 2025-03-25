using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Patrol State")]
public class PatrolStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Movement")]
    public float patrolSpeed = 2f;

    [Header("Transitions")]
    public EnemyStateSO chaseState;

    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, true);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        
        if (distance <= enemy.detectionRange)
        {
            enemy.StateMachine.ChangeState(enemy, chaseState);
            
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0) return;

        Vector3 targetPoint = enemy.patrolPoints[enemy.currentPatrolIndex];
        Vector2 direction = ((Vector2)targetPoint - (Vector2)enemy.transform.position).normalized;

        enemy.rb.velocity = new Vector2(direction.x * patrolSpeed, enemy.rb.velocity.y);
        enemy.UpdateFacingDirection(direction.x);

        float distanceToPoint = Vector2.Distance(enemy.transform.position, targetPoint);
        
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