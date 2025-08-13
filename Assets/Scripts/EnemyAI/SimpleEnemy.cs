using System.Collections;
using Pathfinding;
using UnityEngine;

namespace EnemyAI
{
    public class SimpleEnemy : Enemy
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 3f;
        [SerializeField] private float pointReachThreshold = 0.5f;
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
            currentTargetPosition = playerChannel.CurrentPosition;
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
            if(!CanMove)return;
            if (!playerChannel.IsAlive)
                return;
            

            if (!isReturningHome)
            {
                currentTargetPosition = playerChannel.CurrentPosition;
            }

            float distanceFromHome = Vector2.Distance(rb.position, homePosition);
            bool tooFar = distanceFromHome > maxChaseDistance;
            bool closeEnough = distanceFromHome <= 1.0f;

            if (tooFar && !isReturningHome)
            {
                isReturningHome = true;
                SwitchTarget(homePosition);
            }
            else if (isReturningHome && closeEnough)
            {
                isReturningHome = false;
                SwitchTarget(playerChannel.CurrentPosition);
            }
        }

        private void FixedUpdate()
        {
            if(!CanMove)
            {
                return;
            }
            if (!playerChannel.IsAlive)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            float distanceToPlayer = GetDistanceToPlayer();
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

            Vector2 nextWaypoint = GetSmoothedTarget();
            Vector2 moveDir = (nextWaypoint - rb.position).normalized;

            UpdateFacingDirection(moveDir.x);

            Vector2 desiredVel = moveDir * speed;
            rb.velocity = Vector2.Lerp(rb.velocity, desiredVel, 0.1f);

            if (Vector2.Distance(rb.position, nextWaypoint) <= pointReachThreshold)
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

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 knockDir = ((Vector2)transform.position - contactPoint).normalized;

            rb.velocity = Vector2.zero;
            rb.AddForce(knockDir * recoilForce, ForceMode2D.Force);
            StartCoroutine(StunRoutine());
        }

        private IEnumerator StunRoutine()
        {
            CanMove = false;
            yield return new WaitForSeconds(0.5f);
            CanMove = true;
        }
        private Vector2 GetSmoothedTarget()
        {
            int maxStep = Mathf.Min(5, path.vectorPath.Count - currentPoint - 1);
            Vector2 sum = Vector2.zero;
            for (int i = 0; i <= maxStep; i++)
            {
                sum += (Vector2)path.vectorPath[currentPoint + i];
            }
            return sum / (maxStep + 1);
        }
    }
}
