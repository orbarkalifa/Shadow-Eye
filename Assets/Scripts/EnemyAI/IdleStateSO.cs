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

    
    public override void OnFixedUpdate(EnemyController enemy)
    {
        // Typically no movement in Idle
    }

    public override void OnExit(EnemyController enemy)
    {
    }

    protected override bool Eval(EnemyController enemy)
    {
        return true;
    }
}