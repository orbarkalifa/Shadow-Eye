using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Attack State")]
public class AttackStateSO : EnemyStateSO
{
    [Header("Transitions")]
    [Tooltip("After attacking, if player is still in detection range, go back to chase.")]
    public float detectionRange = 5f;
    public EnemyStateSO chaseState;
    public EnemyStateSO idleOrPatrolState;

    private bool _hasAttacked;

    public override void OnEnter(EnemyController enemy)
    {
        Debug.Log("STATE: Attack -> OnEnter");
        _hasAttacked = false;

        // Perform the immediate attack or trigger an animation
        PerformAttack(enemy);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (!_hasAttacked) return;

        // Decide next state after the attack
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);

        // If still near, go back to chase
        if (distance <= detectionRange)
        {
            enemy.StateMachine.ChangeState(enemy, chaseState);
        }
        else
        {
            // Go to idle or patrol if the player left
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
        Debug.Log("STATE: Attack -> OnExit");
    }

    private void PerformAttack(EnemyController enemy)
    {
        _hasAttacked = true;
        Debug.Log("ENEMY Attack (ScriptableObject State)");
        enemy.LastAttackTime = Time.time;

        // Check if the player is still in range
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
                playerController.TakeDamage(1); // Example damage value
            }
        }
    }
}
