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


    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, true);    
    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);

        if (distanceToPlayer > enemy.detectionRange)
        {
            enemy.StateMachine.ChangeState(enemy, patrolState);
            return;
        }

        if (distanceToPlayer <= enemy.attackRange && 
            Time.time >= enemy.lastAttackTime + enemy.attackCooldown)
        {
            enemy.StateMachine.ChangeState(enemy, attackState);
        }

        if (enemy.currentHits <= 1 && fleeState != null) // Assuming flee threshold is 1 for now
        {
            enemy.StateMachine.ChangeState(enemy, fleeState);
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