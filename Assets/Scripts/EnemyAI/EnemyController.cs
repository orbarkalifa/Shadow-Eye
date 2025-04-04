using Suits;
using UnityEditor;
using UnityEngine;

namespace EnemyAI
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : Character 
    {
        [Header("Basic Settings")]
        public float attackRange = 5f;
        public float attackCooldown = 2f;
        public Transform player;
        public Rigidbody2D rb;
        public Collider2D enemyCollider;

        [HideInInspector] public float lastAttackTime = -Mathf.Infinity;
        public EnemyStateSO startingState;

        [Header("Patrol Points")]
        public Vector3[] patrolPoints;
        [HideInInspector] public int currentPatrolIndex; 
        public float detectionRange = 10f;

        [Header("Suit Drop")]
        [SerializeField] private Suit suitDrop;
        [SerializeField] private ParticleSystem explodeParticles;
        [Header("Vision Settings")]
        [Range(0f, 360f)]
        public float fieldOfViewAngle = 120f;
        public LayerMask obstacleLayerMask;
        public LayerMask playerLayerMask;

        public float CurrentFacingDirection { get; set; } = -1f;

        public EnemyStateMachine StateMachine { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (animator == null) animator = GetComponent<Animator>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (enemyCollider == null) enemyCollider = GetComponent<Collider2D>();

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

            StateMachine = new EnemyStateMachine();
            if (startingState != null)
            {
                StateMachine.Initialize(this, startingState);
            }
            else
            {
                Debug.LogError($"EnemyController ({gameObject.name}): Starting State not set!", this);
                enabled = false;
            }


        }

        private void Update()
        {
            if (StateMachine != null && player != null)
            {
                StateMachine.Update(this);
            }
        }

        private void FixedUpdate()
        {
            if (StateMachine != null && player != null)
            {
                StateMachine.FixedUpdate(this);
            }
        }

        public void UpdateFacingDirection(float xDirection)
        {
            if(Mathf.Sign(xDirection) != CurrentFacingDirection)
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


        public void TriggerAttackDamage()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayerMask);

            foreach (var hitCollider in hitColliders)
            {
                MainCharacter playerController = hitCollider.GetComponent<MainCharacter>();
                if (playerController != null)
                {
                    Debug.Log($"[{gameObject.name}] Attack hit {hitCollider.name}");
                    playerController.TakeDamage(1); // Deal damage

                    break; // Remove if multiple targets could be hit simultaneously
                }
            }
        }
        
        public bool CanSeePlayer()
        {
            if (player == null) return false;

            Vector2 enemyPosition = transform.position;
            Vector2 playerPosition = player.position;
            Vector2 directionToPlayer = (playerPosition - enemyPosition);
            float distanceToPlayer = directionToPlayer.magnitude;

            // 1. Check Distance
            if (distanceToPlayer > detectionRange)
            {
                return false;
            }

            // 2. Check Field of View (FOV)
            // Use the public property CurrentFacingDirection which is now updated correctly
            Vector2 forwardDirection = transform.right * CurrentFacingDirection;
            float angleToPlayer = Vector2.Angle(forwardDirection, directionToPlayer);

            if (angleToPlayer > fieldOfViewAngle / 2f)
            {
#if UNITY_EDITOR
                // Draw FOV lines only when player is potentially in range but outside FOV
                Debug.DrawLine(enemyPosition, enemyPosition + (Vector2)(Quaternion.Euler(0, 0, fieldOfViewAngle / 2f) * forwardDirection * detectionRange), Color.grey);
                Debug.DrawLine(enemyPosition, enemyPosition + (Vector2)(Quaternion.Euler(0, 0, -fieldOfViewAngle / 2f) * forwardDirection * detectionRange), Color.grey);
#endif
                return false;
            }

            // 3. Check Line of Sight (LOS)
            // Combine obstacle and player layers for the raycast check
            int combinedLayerMask = obstacleLayerMask | playerLayerMask;
            RaycastHit2D hit = Physics2D.Raycast(enemyPosition, directionToPlayer.normalized, distanceToPlayer, combinedLayerMask);

#if UNITY_EDITOR
            // Visualize the ray and its result
            if (hit.collider != null)
            {
                // Green if player hit, Red if obstacle hit
                var rayColor = (playerLayerMask == (playerLayerMask | (1 << hit.collider.gameObject.layer))) ? Color.green : Color.red; // Default if nothing hit within range (unlikely)
                Debug.DrawRay(enemyPosition, directionToPlayer.normalized * hit.distance, rayColor); // Draw only up to hit point
            }
            else
            {
                // If nothing hit, draw full length (should mean direct LOS if within range/FOV)
                Debug.DrawRay(enemyPosition, directionToPlayer.normalized * distanceToPlayer, Color.cyan); // Use a different color like cyan
            }
#endif

            // Check if the ray hit anything AND if that thing was on the player layer
            // If hit.collider is null, it means LOS is clear up to distanceToPlayer
            if (hit.collider == null || (playerLayerMask == (playerLayerMask | (1 << hit.collider.gameObject.layer))))
            {
                // Nothing hit OR the first thing hit was the player.
                return true;
            }

            // An obstacle was hit before the player
            return false;
        }
        // --- End Visibility Check ---
        void ActivateDeathParticles()
        {
            explodeParticles = Instantiate(explodeParticles, transform.position, Quaternion.identity);
            explodeParticles.Play();
        }

        protected override void OnDeath()
        {
            ActivateDeathParticles();
            DropSuit();
            StateMachine = null; 
            rb.velocity = Vector2.zero;
            if(enemyCollider) enemyCollider.enabled = false; 
            base.OnDeath();
        }

        private void DropSuit()
        {
            if (suitDrop == null)
            {
                Debug.LogWarning($"No suit assigned to drop for {gameObject.name}.", this);
                return;
            }

            GameObject pickup = suitDrop.suitPrefab != null
                                    ? Instantiate(suitDrop.suitPrefab, transform.position, Quaternion.identity)
                                    : CreateDefaultPickup();

            pickup.tag = "Pickup";

            SuitPickup suitPickup = pickup.GetComponent<SuitPickup>();
            if (suitPickup == null)
            {
                suitPickup = pickup.AddComponent<SuitPickup>();
            }
            suitPickup.Initialize(suitDrop);

            Debug.Log($"{gameObject.name} dropped {suitDrop.suitName}", this);
        }

        private GameObject CreateDefaultPickup()
        {
            GameObject pickup = new GameObject($"{suitDrop.suitName} Pickup");
            pickup.transform.position = transform.position;

            SpriteRenderer sr = pickup.AddComponent<SpriteRenderer>();
            sr.sprite = suitDrop.suitSprite;
            sr.sortingLayerName = "Pickups"; // Example: Assign a sorting layer

            // Physical collider for interaction with ground
            CircleCollider2D physicalCollider = pickup.AddComponent<CircleCollider2D>();
            physicalCollider.radius = 0.3f; // Adjust radius as needed
            physicalCollider.isTrigger = false;

            CircleCollider2D triggerCollider = pickup.AddComponent<CircleCollider2D>();
            triggerCollider.radius = 0.5f; // Make slightly larger than physical
            triggerCollider.isTrigger = true;

            Rigidbody2D suitRb = pickup.AddComponent<Rigidbody2D>();
            suitRb.freezeRotation = true;
            suitRb.gravityScale = 1f; // Give it some gravity

            return pickup;
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Using Handles requires the UnityEditor namespace
            // Draw Patrol Points with interactive handles
            if (patrolPoints is { Length: > 0 })
            {
                Handles.color = Color.yellow; // Handles color for patrol points
                Gizmos.color = Color.yellow; // Gizmos color for lines/spheres

                Vector3 previousPoint = patrolPoints.Length > 0 ? patrolPoints[0] : transform.position; // Start from first point or self
                // Handle position editing only if not playing
                if (!Application.isPlaying)
                {
                    previousPoint = patrolPoints[^1]; // Start with last for line drawing loop
                }


                for (var i = 0; i < patrolPoints.Length; i++)
                {
                    // Use world position if not playing, otherwise relative position might be better if parent moves
                    // For simplicity, let's assume world positions for now.
                    var currentPointWorld = patrolPoints[i];


                    // Draw sphere gizmo at the point's position
                    Gizmos.DrawSphere(currentPointWorld, 0.3f);

                    // Draw lines connecting patrol points (ensure drawing in world space)
                    if (i > 0 || patrolPoints.Length == 1) // Draw line from previous if not first point
                    {
                        Gizmos.DrawLine(previousPoint, currentPointWorld);
                    }
                    // Connect last point to first point if more than one point exists
                    if(i == patrolPoints.Length - 1 && patrolPoints.Length > 1)
                    {
                        Gizmos.DrawLine(currentPointWorld, patrolPoints[0]);
                    }

                    previousPoint = currentPointWorld; // Update previous point for the next iteration


                    // Draw Position Handles only in Editor and when not playing for safety
                    if (!Application.isPlaying)
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector3 newPosition = Handles.PositionHandle(currentPointWorld, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(this, "Move Patrol Point");
                            patrolPoints[i] = newPosition; // Update the point in the array
                        }
                    }

                }
            }

            // Draw Attack Range
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.forward, attackRange); // Use Handles for consistency

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
#endif
    }
}