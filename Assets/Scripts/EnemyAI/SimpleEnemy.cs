using Pathfinding;
using UnityEngine;

namespace EnemyAI
{
    public class SimpleEnemy : Enemy
    {
    
        [SerializeField] float speed;
        [SerializeField] float distanceToNextPoint;
        [SerializeField] CircleCollider2D circleCollider;
        [HideInInspector] public float lastAttackTime = -Mathf.Infinity;
        private Time timer;
        [SerializeField] Seeker seeker;
        Path path;
        int currentPoint;

        protected override void  Awake()
        {
            base.Awake(); 
            InvokeRepeating(nameof(UpdatePath),0f, 0.5f);
 
        }

        private void UpdatePath()
        {
            if(player)
                seeker.StartPath(transform.position, player.position, OnPathComplete);
        }

        private void Update()
        {
            if(!player) return;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if(path == null || distanceToPlayer > detectionRange)
            {
                rb.velocity = Vector2.zero;
                return;
            }
            
            if(currentPoint >= path?.vectorPath.Count)
            {
                Debug.Log("arrived");
                currentPoint = 0;
                rb.velocity = Vector2.zero;
                return;
            }
    
            if(path != null)
            {
                Vector2 direction = ((Vector2)path.vectorPath[currentPoint] - rb.position).normalized;
                rb.AddForce(direction * speed);


                distanceToNextPoint = Vector2.Distance(rb.position, path.vectorPath[currentPoint]);
                if(distanceToNextPoint <= 0.05f)
                    currentPoint++;
            }
        }

        private void OnPathComplete(Path p)
        {
            if(!p.error)
            {
                path = p;
                currentPoint = 0;
            }
        }
    }
}
