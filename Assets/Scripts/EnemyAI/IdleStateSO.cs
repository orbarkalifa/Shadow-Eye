using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Idle State")]
public class IdleStateSO : EnemyStateSO
{
    [Header("Transitions")]
    public float detectionRange = 10f;
    public EnemyStateSO chaseState;
    public float idleDuration = 2f;
    public EnemyStateSO nextStateAfterIdle;

    private float _idleStartTime;

    public override void OnEnter(EnemyController enemy)
    {
        _idleStartTime = Time.time;
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
        // set idle animation
    }

    public override void OnUpdate(EnemyController enemy)
    {
        // Check if player is in range
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (distance <= detectionRange)
        {
            // Transition to chase
            enemy.StateMachine.ChangeState(enemy,chaseState);
            return;
        }

        // If idle time exceeded, transition to the nextStateAfterIdle if assigned
        if (nextStateAfterIdle != null && Time.time >= _idleStartTime + idleDuration)
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