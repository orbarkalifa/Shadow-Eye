using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Attack State")]
public class AttackStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Transitions")]
    public EnemyStateSO chaseState;
    public EnemyStateSO idleOrPatrolState;


    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, false);
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
        // Perform the initial attack
        PerformAttack(enemy);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
    
        // Check if player is out of range and switch state accordingly.
        if (distanceToPlayer > enemy.attackRange)
        {
            if (distanceToPlayer <= enemy.detectionRange)
            {
                enemy.StateMachine.ChangeState(enemy, chaseState);
            }
            else
            {
                enemy.StateMachine.ChangeState(enemy, idleOrPatrolState);
            }
            return;
        }
    
        // Check if the cooldown period has passed before attacking again.
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
        // Record the time of the attack.
        enemy.lastAttackTime = Time.time;
    
        // Attempt to find a player collider within attack range.
        Collider2D playerCollider = Physics2D.OverlapCircle(
            enemy.transform.position,
            enemy.attackRange,
            enemy.playerLayer
        );

        if (playerCollider)
        {
            MainCharacter playerController = playerCollider.GetComponent<MainCharacter>();
            if (playerController)
            {
                playerController.TakeDamage(1);
            }
        }
    }

}
