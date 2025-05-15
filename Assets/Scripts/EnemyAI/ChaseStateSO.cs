using EnemyAI;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Chase State")]
public class ChaseStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Chase Settings")]
    public float chaseSpeed = 4f;

    [Header("Losing Player")]
    public float lostPlayerGracePeriod = 3f; // Time to search before giving up chase
    private float timePlayerLost = -1f; // Using -1f to indicate player not currently lost
    private Vector3 lastKnownPlayerPosition;

    [Header("Transitions")]
    public EnemyStateSO attackState;
    public EnemyStateSO patrolState;
    public EnemyStateSO fleeState;
    public EnemyStateSO returnHomeState;

    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, true);
        timePlayerLost = -1f;
        if (enemy.player != null)
        {
            lastKnownPlayerPosition = enemy.player.position; 
        }
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.player == null) // Safety check: if player is destroyed or null
        {
            enemy.StateMachine.ChangeState(enemy, patrolState); // Or returnHomeState
            return;
        }
        
        // Continuously update last known position as long as we are in chase and player is valid
        lastKnownPlayerPosition = enemy.player.position;

        if (Vector2.Distance(enemy.transform.position, enemy.homePosition) > enemy.maxChaseDistance)
        {
            enemy.StateMachine.ChangeState(enemy, returnHomeState);
            return;
        }

        bool isPlayerBehind = enemy.CheckBehindForPlayer(); // Check once for this frame
        if (isPlayerBehind)
        {
            enemy.Flip(); // Flip if player is behind
        }
        
        bool canCurrentlySeePlayer = enemy.CanSeePlayer();

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
            else if (Time.time - timePlayerLost > lostPlayerGracePeriod) 
            {
                enemy.StateMachine.ChangeState(enemy, patrolState);
                return;
            }
        }
        
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (canCurrentlySeePlayer && distanceToPlayer <= enemy.attackRange &&
            Time.time >= enemy.lastAttackTime + enemy.attackCooldown)
        {
            enemy.StateMachine.ChangeState(enemy, attackState);
            return;
        }

        if (fleeState != null && enemy.currentHits <= 1 && enemy.canFlee) 
        {
            enemy.StateMachine.ChangeState(enemy, fleeState);
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        if (enemy.player == null) // Safety check
        {
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
            return;
        }

        bool canCurrentlySeePlayer = enemy.CanSeePlayer();
        bool isPlayerBehind = enemy.CheckBehindForPlayer(); // Re-check for fixed update responsiveness

        if (canCurrentlySeePlayer || isPlayerBehind) // Player is actively detected
        {
            Vector2 direction = (enemy.player.position - enemy.transform.position).normalized;
            enemy.rb.velocity = new Vector2(direction.x * chaseSpeed, enemy.rb.velocity.y);
            enemy.UpdateFacingDirection(direction.x);
        }
        else if (timePlayerLost >= 0f) // Player is lost, but in grace period
        {
            // Move towards last known position
            Vector2 directionToLastKnown = (lastKnownPlayerPosition - enemy.transform.position).normalized;
            // Stop if very close to last known position to prevent jittering
            if (Vector2.Distance(enemy.transform.position, lastKnownPlayerPosition) > 0.5f) 
            {
                // Optionally, move a bit slower when searching
                enemy.rb.velocity = new Vector2(directionToLastKnown.x * chaseSpeed * 0.75f, enemy.rb.velocity.y); 
                enemy.UpdateFacingDirection(directionToLastKnown.x);
            }
            else
            {
                enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y); // Reached last known, wait/scan
            }
        }
        else // Player truly lost (grace period over or never started chase)
        {
            // This case should ideally be handled by state transition in OnUpdate.
            // If somehow still in chase state and player not seen (e.g. initial frame before OnUpdate runs), stop.
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y); 
        }
    }

    public override void OnExit(EnemyController enemy)
    {
        // Stop horizontal movement when exiting chase state.
        // The next state's OnEnter should handle animations (e.g., setting isWalking if patrolling).
        enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
    }
}