
using Player;
using UnityEngine;
namespace EnemyAI
{
    public class IraController : EnemyController
    {
        public override void TriggerAttackDamage()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayerMask);
            foreach(var hitCollider in hitColliders)
            {
                if(hitCollider.gameObject.CompareTag("Player") && !playerChannel.IsInvincible)
                {
                    playerChannel.DealDamage(1, transform.position);
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
            base.Patrol();
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



    }
}