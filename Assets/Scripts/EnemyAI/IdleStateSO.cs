using EnemyAI;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Idle State")]
public class IdleStateSO : EnemyStateSO
{
    [Header("Transitions")]
    public EnemyStateSO chaseState;
    public float idleDuration = 2f;
    public EnemyStateSO nextStateAfterIdle;

    private float idleStartTime;

    public override void OnEnter(EnemyController enemy)
    {
        idleStartTime = Time.time;
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
        enemy.animator.SetBool("isWalking", false);    }

    public override void OnUpdate(EnemyController enemy)
    {
        // Check if player is in range
        float distance = enemy.GetDistanceToPlayer();
        if (distance <= enemy.detectionRange)
        {
            // Transition to chase
            enemy.StateMachine.ChangeState(enemy,chaseState);
            return;
        }

        // If idle time exceeded, transition to the nextStateAfterIdle if assigned
        if (nextStateAfterIdle != null && Time.time >= idleStartTime + idleDuration)
        {
            enemy.StateMachine.ChangeState(enemy,nextStateAfterIdle);
        }
    }
    
    public override void OnFixedUpdate(EnemyController enemy)
    {
        // Typically no movement in Idle
    }

    public override void OnExit(EnemyController enemy)
    {
    }
}