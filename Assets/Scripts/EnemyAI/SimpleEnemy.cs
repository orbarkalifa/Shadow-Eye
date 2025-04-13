using System;
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
        private int currentPoint ;
 
        [HideInInspector] public float lastAttackTime = -Mathf.Infinity;

        protected override void Awake()
        {
            base.Awake();
            pathfindingTarget = player.transform;
            StartCoroutine(RecalculatePath());
        }

        private IEnumerator RecalculatePath()
        {
            while (player != null)
            {
                seeker.StartPath(transform.position, pathfindingTarget.position, OnPathComplete);
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
            if(Vector2.Distance(homePosition, rb.position) > maxChaseDistance)
            {
                pathfindingTarget.position = homePosition;
            }

            if(rb.position == homePosition)
            {
                pathfindingTarget = player.transform;
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
            if (path == null || distanceToPlayer > detectionRange)
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

            var distanceToNextPoint = Vector2.Distance(rb.position, nextWaypoint);
            if (distanceToNextPoint <= pointReachThreshold)
            {
                currentPoint++;
            }
        }
    }
}
