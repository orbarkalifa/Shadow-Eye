using EnemyAI;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Patrol State")]
public class PatrolStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Movement")] // How close to be to consider "arrived"
    public float patrolSpeed = 2f;
    public float waypointArrivalThreshold = 0.5f; 
    [Header("Transitions")]
    public EnemyStateSO chaseState;

    public override void OnEnter(EnemyController enemy)
    {
        enemy.animator.SetBool(isWalking, true);
        // Ensure enemy faces and starts moving towards its patrol point immediately.
        enemy.Patrol();
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.CheckBehindForPlayer() || enemy.CanSeePlayer())
        {
            enemy.StateMachine.ChangeState(enemy, chaseState);
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        // If transitions occur in OnUpdate, this FixedUpdate will execute for the current state.
        // If player is seen, OnUpdate will handle the state change.
        // ChaseState's OnEnter/OnFixedUpdate will take over movement.
        enemy.Patrol();
    }

    private void MoveTowardsPatrolPoint(EnemyController enemy)
    {
        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0)
        {
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y); // Stop if no patrol points
            return;
        }

        Vector3 currentTargetPoint = enemy.patrolPoints[enemy.currentPatrolIndex];
        float distanceToCurrentTarget = Vector2.Distance(enemy.transform.position, currentTargetPoint);

        // Check if we need to switch to the next patrol point
        if (distanceToCurrentTarget <= waypointArrivalThreshold)
        {
            enemy.currentPatrolIndex = (enemy.currentPatrolIndex + 1) % enemy.patrolPoints.Length;
            currentTargetPoint = enemy.patrolPoints[enemy.currentPatrolIndex]; // Update to the new target
            // Optionally, add a small pause here (e.g. with a timer) if desired.
        }

        // Move towards the (potentially new) current target point
        Vector2 direction = ((Vector2)currentTargetPoint - (Vector2)enemy.transform.position).normalized;

        if (direction.sqrMagnitude > 0.01f) // If there's a direction to move (not already at target)
        {
            enemy.rb.velocity = new Vector2(direction.x * patrolSpeed, enemy.rb.velocity.y);
            enemy.UpdateFacingDirection(direction.x);
        }
        else
        {
            // Very close or at the target, stop to prevent jitter.
            enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
        }
    }

    public override void OnExit(EnemyController enemy)
    {
        // Stop horizontal movement when exiting patrol
        enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
    }
}