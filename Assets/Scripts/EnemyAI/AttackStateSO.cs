using EnemyAI;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "EnemyAI/States/Attack State")]
public class AttackStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Transitions")]
    public EnemyStateSO chaseState;
    public EnemyStateSO patrolState;
    public EnemyStateSO fleeState; 

    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, false);
        Debug.Log("Entered attack state");
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
        enemy.Attack();
    }

    public override void OnUpdate(EnemyController enemy)
    {
        base.OnUpdate(enemy);
        if (Time.time >= enemy.lastAttackTime + enemy.attackCooldown)
        {
            enemy.Attack();
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
    }

    public override void OnExit(EnemyController enemy)
    {
        // Optionally reset or clean up any attack state here.
    }

    protected override bool Eval(EnemyController enemy)
    {
        return enemy.CanSeePlayer() && enemy.GetDistanceToPlayer() <= enemy.attackRange
                                    && Time.time >= enemy.lastAttackTime + enemy.attackCooldown;
    }
}
