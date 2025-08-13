using UnityEngine;
using Player;
using UnityEditor;

namespace EnemyAI
{
    public class DuriController : EnemyController
    {
        [SerializeField]
        private float attackHeight = 4f;
        [SerializeField]
        private float bounceForce = 10f;

        public override void TriggerAttackDamage()
        {
            // The enemy's world position is the origin for the attack.
            Vector2 originPoint = rb.position;

            // If attackRange is the distance from the center to the edge,
            // the total width of the box is attackRange * 2.
            Vector2 hitboxSize = new Vector2(attackRange * 2f, attackHeight);

            // Detect players inside this centered attack box
            Collider2D[] hits = Physics2D.OverlapBoxAll(originPoint, hitboxSize, 0f, playerLayerMask);
            foreach(var col in hits)
            {
                if(col.CompareTag("Player"))
                {
                    // The recoil direction is still based on the player's relative position.
                    // This part of your logic was already good.
                    var forceDirection = playerChannel.CurrentPosition - new Vector2(0, bounceForce);
                    playerChannel.DealDamage(2,forceDirection);

                }
            }
        }

        public override void Attack()
        {
            lastAttackTime = Time.time;
            animator.CrossFadeInFixedTime("Duri_Attack", 0.05f);
        }

        public override void Patrol()
        {
            base.Patrol();
            Vector3 currentTargetPoint = patrolPoints[currentPatrolIndex];
            float distanceToCurrentTarget = Vector2.Distance(transform.position, currentTargetPoint);

            // Check if we need to switch to the next patrol point
            if(distanceToCurrentTarget <= waypointArrivalThreshold)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                currentTargetPoint = patrolPoints[currentPatrolIndex]; // Update to the new target
                // Optionally, add a small pause here (e.g. with a timer) if desired.
            }

            // Move towards the (potentially new) current target point
            Vector2 direction = ((Vector2)currentTargetPoint - (Vector2)transform.position).normalized;

            if(direction.sqrMagnitude > 0.01f) // If there's a direction to move (not already at target)
            {
                rb.velocity = new Vector2(direction.x * patrolSpeed, rb.velocity.y);
                UpdateFacingDirection(direction.x);
            }
            else
            {
                // Very close or at the target, stop to prevent jitter.
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        
#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            // --- These calculations MUST match TriggerAttackDamage() ---
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>(); // Try to get the component for editor drawing.
                if (rb == null) return; // If it's still null, exit to prevent errors.
            }
            
            // 1. Define the center point of the attack hitbox.
            // It's centered on the Rigidbody's position 
            Vector2 hitboxCenter = rb.position;

            // 2. Define the size of the hitbox.
            // The width is doubled to extend on both sides.
            Vector2 hitboxSize = new Vector2(attackRange * 2f, attackHeight);

            // 3. Draw the Gizmo
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(hitboxCenter, hitboxSize);
        }
#endif
    }
}