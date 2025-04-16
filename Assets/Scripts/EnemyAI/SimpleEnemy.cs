using System.Collections;
using Pathfinding;
using UnityEngine;

namespace EnemyAI
{
    public class SimpleEnemy : Enemy
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 3f;
        [SerializeField] private float pointReachThreshold = 0.1f;
        [SerializeField] private float pathUpdateInterval = 0.5f;
        [SerializeField] private CircleCollider2D circleCollider;

        [Header("Pathfinding")]
        [SerializeField] private Transform pathfindingTarget;
        [SerializeField] private Seeker seeker;
        private Path path;
        private int currentPoint;
        private bool isReturningHome;
        private Vector2 currentTargetPosition;
        
        protected override void Awake()
        {
            base.Awake();
            currentTargetPosition = player.position;
            StartCoroutine(RecalculatePath());
        }

        private IEnumerator RecalculatePath()
        {
            while (true)
            {
                seeker.StartPath(transform.position, currentTargetPosition, OnPathComplete);
                yield return new WaitForSeconds(pathUpdateInterval);
            }
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentPoint = 0;
            }
        }

        protected void Update()
        {
            // Only check for the player; homePosition is a value type and does not need null checking.
            if (player == null)
                return;

            // When not returning home, update the target position to the player's current position.
            if (!isReturningHome)
            {
                currentTargetPosition = player.position;
            }

            float distanceFromHome = Vector2.Distance(rb.position, homePosition);
            bool tooFar = distanceFromHome > maxChaseDistance;
            // Adjusted threshold: consider "close enough" if within 1 unit.
            bool closeEnough = distanceFromHome <= 1.0f;

            // If too far from home, switch target to home.
            if (tooFar && !isReturningHome)
            {
                isReturningHome = true;
                SwitchTarget(homePosition);
            }
            // If returning home and close enough, resume chasing the player.
            else if (isReturningHome && closeEnough)
            {
                isReturningHome = false;
                SwitchTarget(player.position);
            }
        }

        private void FixedUpdate()
        {
            if (player == null)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (path == null || (distanceToPlayer > detectionRange && !isReturningHome))
            {
                rb.velocity = Vector2.zero;
                return;
            }

            if (currentPoint >= path.vectorPath.Count)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            Vector2 nextWaypoint = path.vectorPath[currentPoint];
            Vector2 direction = (nextWaypoint - rb.position).normalized;
            Vector2 desiredVelocity = direction * speed;

            rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, 0.1f);

            float distanceToNextPoint = Vector2.Distance(rb.position, nextWaypoint);
            if (distanceToNextPoint <= pointReachThreshold)
            {
                currentPoint++;
            }
        }

        private void SwitchTarget(Vector2 newTargetPosition)
        {
            Debug.Log("Switching to " + newTargetPosition);
            currentTargetPosition = newTargetPosition;
            seeker.StartPath(transform.position, currentTargetPosition, OnPathComplete);
        }
    }
}
