
using Player;
using UnityEngine;
namespace EnemyAI
{
    public class IraController : EnemyController
    {
        public override void TriggerAttackDamage()
        {
            float recoilDirection;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayerMask);
            foreach(var hitCollider in hitColliders)
            {
                PlayerController playerController = hitCollider.GetComponent<PlayerController>();
                if(playerController != null && !playerController.IsInvincible)
                {
                    recoilDirection = GetRecoilDirection(playerController.transform);
                    playerController.TakeDamage(1, recoilDirection);
                    break;
                }
            }
        }

        public override void Attack()
        {
            lastAttackTime = Time.time;
            animator.CrossFadeInFixedTime("Ira_attack", 0.05f);
        }

        public override void Patrol()
        {
            if(!CanMove || isStunned) return;

            if(patrolPoints == null || patrolPoints.Length == 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }

            Vector3 currentTargetPoint = patrolPoints[currentPatrolIndex];
    
            // --- FIX: Use a more robust horizontal distance check ---
            float distanceToCurrentTarget = Mathf.Abs(transform.position.x - currentTargetPoint.x);

            if(distanceToCurrentTarget <= waypointArrivalThreshold)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }

            Vector2 direction = ((Vector2)patrolPoints[currentPatrolIndex] - (Vector2)transform.position).normalized;

            if(direction.sqrMagnitude > 0.01f)
            {
                rb.velocity = new Vector2(direction.x * patrolSpeed, rb.velocity.y);
                UpdateFacingDirection(direction.x);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        public override void Chase()
        {
            if(!CanMove || isStunned || player == null)
            {
                if(player == null) rb.velocity = new Vector2(0, rb.velocity.y); // Stop if player is gone
                return;
            }

            // Determine if we should react to player behind us (optional quick turn)
            if(CheckBehindForPlayer() && !CanSeePlayer()) // Prioritize actual sight if available
            {
                Flip(); // This updates CurrentFacingDirection
            }

            Vector3 targetPosition;
            bool currentlySeesPlayer = CanSeePlayer(); // Cache for this frame

            if(currentlySeesPlayer)
            {
                targetPosition = player.position; // lastKnownPlayerPosition is updated by CanSeePlayer
            }
            else
            {
                targetPosition = lastKnownPlayerPosition;
            }

            Vector2 directionToTarget = ((Vector2)targetPosition - (Vector2)transform.position);
            float distanceToTarget = directionToTarget.magnitude; // Get actual distance

            // If not seeing player and have arrived at LKP, stop. State machine will handle transition.
            // Use a small threshold to prevent jitter.
            if(!currentlySeesPlayer
               && distanceToTarget < waypointArrivalThreshold * 0.5f) // Or a dedicated LKP arrival threshold
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(directionToTarget.normalized.x * chaseSpeed, rb.velocity.y);
            }

            // Update facing direction based on movement or target direction
            if(Mathf.Abs(directionToTarget.normalized.x) > 0.01f)
            {
                UpdateFacingDirection(directionToTarget.normalized.x);
            }
        }
        
        public override void Flee()
        {
            if(!CanMove || isStunned)
            {
                return;
            }

            Vector2 directionToPlayer = player.position - transform.position;
            Vector2 fleeDirection = -directionToPlayer.normalized;
            rb.velocity = new Vector2(fleeDirection.x * fleeSpeed, rb.velocity.y);
            UpdateFacingDirection(fleeDirection.x);
        }

        public override void ReturnHome()
        {
            if(!CanMove || isStunned)
            {
                return;
            }

            Vector2 dir = (homePosition - (Vector2)transform.position).normalized;
            rb.velocity = new Vector2(dir.x * returnSpeed, rb.velocity.y);
            UpdateFacingDirection(dir.x);
        }

    }
}