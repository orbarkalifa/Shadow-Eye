using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Idle State")]
public class IdleStateSO : EnemyStateSO
{
    [Header("Transitions")]
    [Tooltip("If the player is within this range, we go to 'chaseState'.")]
    public float detectionRange = 5f;

    [Tooltip("Which state to switch to if the player is detected?")]
    public EnemyStateSO chaseState;

    [Tooltip("Optional: After some idle time, go to this state (e.g. patrol).")]
    public float idleDuration = 2f;
    public EnemyStateSO nextStateAfterIdle;

    private float _idleStartTime;

    public override void OnEnter(EnemyController enemy)
    {
        Debug.Log("STATE: Idle -> OnEnter");
        _idleStartTime = Time.time;
        // Stop horizontal movement
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
        // Possibly set idle animation
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
        Debug.Log("STATE: Idle -> OnExit");
    }
}