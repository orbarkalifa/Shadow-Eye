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
            if(!canMove)return;
            if (player == null)
                return;
            

            if (!isReturningHome)
            {
                currentTargetPosition = player.position;
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
                SwitchTarget(player.position);
            }
        }

        private void FixedUpdate()
        {
            if(!canMove)
            {
                return;
            }
            if (player == null)
            {
                rb.velocity = Vector2.zero;
                return;
            }



            float distanceToPlayer = Vector2.Distance(rb.position, player.position);
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
            canMove = false;
            yield return new WaitForSeconds(0.5f);
            canMove = true;
        }
    }
}
