using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Attack State")]
public class AttackStateSO : EnemyStateSO
{
    [Header("Transitions")]
    public float detectionRange = 10f;
    public EnemyStateSO chaseState;
    public EnemyStateSO idleOrPatrolState;

    private bool _hasAttacked;

    public override void OnEnter(EnemyController enemy)
    {
        _hasAttacked = false;

        performAttack(enemy);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (!_hasAttacked) return;
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (distance <= detectionRange)
        {
            enemy.StateMachine.ChangeState(enemy, chaseState);
        }
        else
        {
            enemy.StateMachine.ChangeState(enemy, idleOrPatrolState);
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        // Typically no movement during Attack
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
    }

    public override void OnExit(EnemyController enemy)
    {
    }

    private void performAttack(EnemyController enemy)
    {
        _hasAttacked = true;
        enemy.LastAttackTime = Time.time;

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
