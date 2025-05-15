using EnemyAI;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "EnemyAI/States/Chase State")]
public class ChaseStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Chase Settings")]
    public float chaseSpeed = 4f;

    [Header("Transitions")]
    public EnemyStateSO attackState;
    public EnemyStateSO patrolState;
    public EnemyStateSO fleeState;
    public EnemyStateSO returnHomeState;


    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, true);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (Vector2.Distance(enemy.transform.position, enemy.homePosition) > enemy.maxChaseDistance)
        {
            enemy.StateMachine.ChangeState(enemy, returnHomeState);
            return;
        }
        if (!enemy.CanSeePlayer() || enemy.CheckBehindForPlayer())
        {
            enemy.StateMachine.ChangeState(enemy, patrolState);
            return; 
        }
        
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (distanceToPlayer <= enemy.attackRange &&
            Time.time >= enemy.lastAttackTime + enemy.attackCooldown)
        {
            enemy.StateMachine.ChangeState(enemy, attackState);
            return;
        }

        if (fleeState != null && enemy.currentHits <= 1 && enemy.canFlee) 
        {
            enemy.StateMachine.ChangeState(enemy, fleeState);
        }

    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        if (!enemy.CanSeePlayer())
        {
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y); 
            return; 
        }


        Vector2 direction = (enemy.player.position - enemy.transform.position).normalized;
        enemy.rb.velocity = new Vector2(direction.x * chaseSpeed, enemy.rb.velocity.y);
        enemy.UpdateFacingDirection(direction.x);
    }

    public override void OnExit(EnemyController enemy)
    {
        // Stop walking animation when exiting chase (unless next state sets it)
        // Consider if Attack/Flee/Patrol immediately set it again. If so, this might not be needed.
        // enemy.animator.SetBool(isWalking, false); 

        // Important: Stop horizontal movement when exiting chase state
        enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
    }
}