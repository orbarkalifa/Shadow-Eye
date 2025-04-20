using EnemyAI;
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
        MoveTowardsPatrolPoint(enemy); 
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if(enemy.CheckBehindForPlayer()) enemy.Flip();
        if (enemy.CanSeePlayer())
        {
            if (chaseState)
            {
                enemy.StateMachine.ChangeState(enemy, chaseState);
            }
            else
            {
                Debug.LogWarning($"PatrolStateSO on {enemy.gameObject.name} missing Chase State transition!");
            }
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        if (enemy.CanSeePlayer() || enemy.CheckBehindForPlayer())
        {
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
            return;
        }
        MoveTowardsPatrolPoint(enemy);
    }

    private void MoveTowardsPatrolPoint(EnemyController enemy)
    {
        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0) return;

        Vector3 targetPoint = enemy.patrolPoints[enemy.currentPatrolIndex];
        Vector2 direction = ((Vector2)targetPoint - (Vector2)enemy.transform.position).normalized;

        var distanceToPoint = Vector2.Distance(enemy.transform.position, targetPoint);
        if (distanceToPoint > 1f) 
        {
            enemy.rb.velocity = new Vector2(direction.x * patrolSpeed, enemy.rb.velocity.y);
            enemy.UpdateFacingDirection(direction.x);
        }
        else 
        {
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);

            enemy.currentPatrolIndex++;
            if (enemy.currentPatrolIndex >= enemy.patrolPoints.Length)
            {
                enemy.currentPatrolIndex = 0;
            }
             targetPoint = enemy.patrolPoints[enemy.currentPatrolIndex];
             direction = ((Vector2)targetPoint - (Vector2)enemy.transform.position).normalized;
             enemy.UpdateFacingDirection(direction.x); 
        }
    }


    public override void OnExit(EnemyController enemy)
    {
        // Optional: Stop movement when exiting patrol
        // enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
    }
}