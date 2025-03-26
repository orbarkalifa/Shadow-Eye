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
        PerformAttack(enemy);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
    
        if (distanceToPlayer > enemy.attackRange)
        {
            if (distanceToPlayer <= enemy.detectionRange)
            {
                enemy.StateMachine.ChangeState(enemy, chaseState);
            }
            else
            {
                enemy.StateMachine.ChangeState(enemy, patrolState);
            }
            return;
        }
    
        if (enemy.currentHits <= 1 && fleeState != null) // Assuming flee threshold is 1 for now
        {
            enemy.StateMachine.ChangeState(enemy, fleeState);
        }
        
        if (Time.time >= enemy.lastAttackTime + enemy.attackCooldown)
        {
            PerformAttack(enemy);
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

    private void PerformAttack(EnemyController enemy)
    {
        enemy.lastAttackTime = Time.time;
    
        Collider2D playerCollider = Physics2D.OverlapCircle(
            enemy.transform.position,
            enemy.attackRange,
            enemy.playerLayer
        );

        if (playerCollider)
        {
            enemy.animator.CrossFadeInFixedTime("Ira_attack", 0.05f);
            MainCharacter playerController = playerCollider.GetComponent<MainCharacter>();
            if (playerController)
            {
                playerController.TakeDamage(1);
            }
        }
    }

}
