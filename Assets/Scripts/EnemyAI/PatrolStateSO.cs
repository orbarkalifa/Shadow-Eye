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
        if(CheckBehindForPlayer(enemy)) enemy.Flip();
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
        if (enemy.CanSeePlayer() || CheckBehindForPlayer(enemy))
        {
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
            return;
        }
        MoveTowardsPatrolPoint(enemy);
    }
 private bool CheckBehindForPlayer(EnemyController enemy)
    {
        if (!enemy.player) return false;

        Vector2 enemyPosition = enemy.transform.position;
        Vector2 playerPosition = enemy.player.position;

        float distanceToPlayer = Vector2.Distance(enemyPosition, playerPosition);
        if (distanceToPlayer > enemy.detectionRange)
        {
            return false;
        }

        Vector2 backDirection = -enemy.transform.right * enemy.CurrentFacingDirection;
        int combinedLayerMask = enemy.obstacleLayerMask | enemy.playerLayerMask;

        RaycastHit2D hit = Physics2D.Raycast(
            enemyPosition,
            backDirection,
            enemy.detectionRange,
            combinedLayerMask
        );

        #if UNITY_EDITOR
        Color rayColor = Color.magenta; 
        if (hit.collider)
        {
            rayColor = (enemy.playerLayerMask == (enemy.playerLayerMask | (1 << hit.collider.gameObject.layer))) ? Color.green : Color.red;
            Debug.DrawRay(enemyPosition, backDirection * hit.distance, rayColor);
        }
        else
        {
            Debug.DrawRay(enemyPosition, backDirection * enemy.detectionRange, rayColor * 0.5f); 
        }
        #endif

        if (hit.collider && (enemy.playerLayerMask == (enemy.playerLayerMask | (1 << hit.collider.gameObject.layer))))
        {
            return true;
        }

        return false;
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