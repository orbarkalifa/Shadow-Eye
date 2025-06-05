using EnemyAI;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Chase State")]
public class ChaseStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    

    [Header("Losing Player")]
 // Using -1f to indicate player not currently lost
    

    [Header("Transitions")]
    public EnemyStateSO attackState;
    public EnemyStateSO patrolState;
    public EnemyStateSO fleeState;
    public EnemyStateSO returnHomeState;

    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, true);
        enemy.timePlayerLost = -1f;
    }
    
    public override void OnFixedUpdate(EnemyController enemy)
    {
        if (enemy.player == null) // Safety check
        {
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
        }
        enemy.Chase();
    }

    public override void OnExit(EnemyController enemy)
    {
        // Stop horizontal movement when exiting chase state.
        // The next state's OnEnter should handle animations (e.g., setting isWalking if patrolling).
        enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
    }

    protected override bool Eval(EnemyController enemy)
    {
        return enemy.CanSeePlayer() || enemy.CheckBehindForPlayer();
    }
}