using System;
using UnityEditor;
using UnityEngine;

namespace EnemyAI
{
    public abstract class Enemy : Character
    {
        [Header("Vision Settings")]
        [Range(0f, 360f)]
        public float fieldOfViewAngle = 120f;
        public float recoilForce = 100;
        public Rigidbody2D rb;
        public Transform player;
        public float detectionRange = 10f;
        public LayerMask obstacleLayerMask;
        public LayerMask playerLayerMask;

        protected override void Awake()
        {
            base.Awake();
            if (rb == null) rb = GetComponent<Rigidbody2D>();

            if (player == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    player = playerObject.transform;
                }
                else
                {
                    Debug.LogError($"EnemyController ({gameObject.name}): Player not found! Make sure player has 'Player' tag or is assigned.", this);
                    enabled = false;
                    return;
                }
            }
        }

        public void UpdateFacingDirection(float xDirection)
        {
            if(!Mathf.Approximately(Mathf.Sign(xDirection), CurrentFacingDirection))
            {
                Flip();
            }
        }
        public void Flip()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            CurrentFacingDirection = -CurrentFacingDirection;
        }
        
        public bool CanSeePlayer()
        {
            if (player == null) return false;

            Vector2 enemyPosition = transform.position;
            Vector2 playerPosition = player.position;
            Vector2 directionToPlayer = (playerPosition - enemyPosition);
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer > detectionRange)
            {
                return false;
            }

            Vector2 forwardDirection = transform.right * CurrentFacingDirection;
            float angleToPlayer = Vector2.Angle(forwardDirection, directionToPlayer);

            if (angleToPlayer > fieldOfViewAngle / 2f)
            {
#if UNITY_EDITOR
                Debug.DrawLine(enemyPosition, enemyPosition + (Vector2)(Quaternion.Euler(0, 0, fieldOfViewAngle / 2f) * forwardDirection * detectionRange), Color.grey);
                Debug.DrawLine(enemyPosition, enemyPosition + (Vector2)(Quaternion.Euler(0, 0, -fieldOfViewAngle / 2f) * forwardDirection * detectionRange), Color.grey);
#endif
                return false;
            }

            int combinedLayerMask = obstacleLayerMask | playerLayerMask;
            RaycastHit2D hit = Physics2D.Raycast(enemyPosition, directionToPlayer.normalized, distanceToPlayer, combinedLayerMask);

#if UNITY_EDITOR
            if (hit.collider != null)
            {
                var rayColor = (playerLayerMask == (playerLayerMask | (1 << hit.collider.gameObject.layer))) ? Color.green : Color.red; // Default if nothing hit within range (unlikely)
                Debug.DrawRay(enemyPosition, directionToPlayer.normalized * hit.distance, rayColor); // Draw only up to hit point
            }
            else
            {
                Debug.DrawRay(enemyPosition, directionToPlayer.normalized * distanceToPlayer, Color.cyan); // Use a different color like cyan
            }
#endif

            if (hit.collider == null || (playerLayerMask == (playerLayerMask | (1 << hit.collider.gameObject.layer))))
            {
                return true;
            }

            return false;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 recoilDirection = Vector2.zero;
            if(collision.gameObject.CompareTag("Player"))
            {
                MainCharacter player = collision.gameObject.GetComponent<MainCharacter>();
                if(recoilDirection.x !=0)
                    recoilDirection = recoilDirection.x>0 ? Vector2.right : Vector2.left;
                else
                {
                    recoilDirection = Vector2.up;
                }
                player.TakeDamage(1,recoilDirection);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw Detection Range
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(transform.position, Vector3.forward, detectionRange);
            
            // Draw FOV lines if selected
            if (CanSeePlayer()) // Example condition, or always draw if selected
            {
                Vector2 enemyPosition = transform.position;
                Vector2 forward = transform.right * CurrentFacingDirection;
                Handles.color = new Color(1f, 1f, 0f, 0.2f); // Semi-transparent yellow
                Handles.DrawSolidArc(enemyPosition, Vector3.forward, Quaternion.Euler(0,0,-fieldOfViewAngle/2f) * forward, fieldOfViewAngle, detectionRange);
            }
        }

        public override void TakeDamage(int damage, Vector2 direction)
        {
            rb.velocity = Vector2.zero;
            Debug.Log($"added recoil{direction*recoilForce}");
            rb.AddForce(direction*recoilForce , ForceMode2D.Impulse);
            base.TakeDamage(damage);
            
        }
    }
}
