using System;
using EnemyAI;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Chase State")]
public class ChaseStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    

    [Header("Losing Player")]
    public float lostPlayerGracePeriod = 3f; // Time to search before giving up chase
    private float timePlayerLost = -1f; // Using -1f to indicate player not currently lost
    

    [Header("Transitions")]
    public EnemyStateSO attackState;
    public EnemyStateSO patrolState;
    public EnemyStateSO fleeState;
    public EnemyStateSO returnHomeState;

    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, true);
        timePlayerLost = -1f;
    }

    public override void OnUpdate(EnemyController enemy)
    {
        // Continuously update last known position as long as we are in chase and player is valid
        if (Vector2.Distance(enemy.transform.position, enemy.homePosition) > enemy.maxChaseDistance)
        {
            enemy.StateMachine.ChangeState(enemy, returnHomeState);
            return;
        }

        bool isPlayerBehind = enemy.CheckBehindForPlayer(); // Check once for this frame
        bool canCurrentlySeePlayer = enemy.CanSeePlayer();

        
        float distanceToPlayer = enemy.GetDistanceToPlayer();
        if (canCurrentlySeePlayer && distanceToPlayer <= enemy.attackRange &&
            Time.time >= enemy.lastAttackTime + enemy.attackCooldown)
        {
            enemy.StateMachine.ChangeState(enemy, attackState);
            return;
        }
        
        if (canCurrentlySeePlayer || isPlayerBehind) 
        {
            timePlayerLost = -1f;
        }
        else 
        {
            if (timePlayerLost < 0f) 
            {
                timePlayerLost = Time.time;
            }
            else if (Time.time - timePlayerLost > lostPlayerGracePeriod || Math.Abs(enemy.transform.position.x - enemy.lastKnownPlayerPosition.x) <= enemy.waypointArrivalThreshold) 
            {
                enemy.StateMachine.ChangeState(enemy, patrolState);
                return;
            }
        }

        if (fleeState != null && enemy.currentHits <= 1 && enemy.canFlee) 
        {
            enemy.StateMachine.ChangeState(enemy, fleeState);
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        enemy.Chase();
    }

    public override void OnExit(EnemyController enemy)
    {
        // Stop horizontal movement when exiting chase state.
        // The next state's OnEnter should handle animations (e.g., setting isWalking if patrolling).
        enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
    }
}