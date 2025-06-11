using UnityEditor;
using Suits;
using UnityEngine;
using GameStateManagement;

namespace EnemyAI
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class EnemyController : Enemy
    {
        [Header("Basic Settings")]
        public float attackRange;
        public float attackCooldown;
        public Collider2D enemyCollider;
        protected static bool hasShownSuitTutorial;

        [HideInInspector] public float lastAttackTime = -Mathf.Infinity;
        public EnemyStateSO startingState;
        
        [Header("Chase Settings")]
        public float chaseSpeed;
        
        [Header("Patrol Settings")] // Changed header slightly
        public float patrolSpeed = 2f;
        public float waypointArrivalThreshold = 0.5f;
        public Vector3[] patrolPoints; // Keep this existing array
        [HideInInspector] public int currentPatrolIndex;

        // --- NEW SETTINGS FOR DYNAMIC 2-POINT PATROL ---
        [Header("Dynamic Two-Point Patrol (Overrides patrolPoints[0] & [1])")]
        [Tooltip("If true, patrolPoints[0] and [1] will be dynamically found via raycasts.")]
        public bool useDynamicTwoPointPatrol = true; // Set to true for enemies that should use this
        [Tooltip("Max distance to raycast left/right to find patrol boundaries.")]
        public float patrolBoundDetectionDistance = 20;
        [Tooltip("Layer mask for objects that define patrol boundaries (e.g., Walls, Ground).")]
        public LayerMask whatIsPatrolBoundary;
        
        [Header("Flee Settings")]
        public bool canFlee = true;
        public float fleeSpeed;
        public float fleeDistance;
        
        [Header("Return Home Settings")]
        public float returnSpeed;
        [Header("Suit Drop")]
        [SerializeField] protected Suit suitDrop;
        
        public EnemyStateMachine StateMachine { get; private set; }

        public abstract void TriggerAttackDamage();
        public abstract void Attack();
        public abstract void Patrol();
        //public void Idle();
        public abstract void Chase();
        public abstract void Flee();
        public abstract void ReturnHome();
        
        protected override void Awake()
        {
            base.Awake();
            if (enemyCollider == null) enemyCollider = GetComponent<Collider2D>();
            
            StateMachine = new EnemyStateMachine();
            
            if (useDynamicTwoPointPatrol)
            {
                InitializeDynamicPatrolPoints();
            }
            else if (patrolPoints == null || patrolPoints.Length == 0)
            {
                // Optional: Fallback for non-dynamic patrol if no points are set.
                // Could default to a small patrol around homePosition.
                // Debug.LogWarning($"{gameObject.name}: Patrol enabled but no patrol points set and dynamic patrol is off. Patrolling around home.");
                // patrolPoints = new Vector3[] { homePosition + Vector2.left, homePosition + Vector2.right };
            }
            
            hasShownSuitTutorial = false;
            if (startingState != null)
            {
                // If currentPatrolIndex is not set by InitializeDynamicPatrolPoints (e.g., useDynamic is false)
                // and patrolPoints exist, ensure currentPatrolIndex is valid.
                if (patrolPoints != null && patrolPoints.Length > 0 && !useDynamicTwoPointPatrol) {
                    currentPatrolIndex = 0; 
                }
                StateMachine.Initialize(this, startingState);
            }
            else
            {
                Debug.LogError($"EnemyController ({gameObject.name}): Starting State not set!", this);
                enabled = false;
            }
        }
                
        
        private void InitializeDynamicPatrolPoints()
        {
            // Ensure Rigidbody and Collider are available. Awake() should have handled this.
            if (rb == null || enemyCollider == null)
            {
                Debug.LogError($"{gameObject.name}: Rigidbody2D or Collider2D not found. Dynamic patrol point initialization failed.", this);
                useDynamicTwoPointPatrol = false; // Disable this feature to prevent further errors.
                return;
            }

            // --- BEST PRACTICE: CALCULATE RAYCAST ORIGIN FROM "FEET" ---

            // 1. Define a small buffer to lift the raycast slightly off the ground.
            const float verticalBuffer = 0.1f;

            // 2. Calculate the "feet" position using the collider's bottom edge + buffer.
            float feetY = enemyCollider.bounds.min.y + verticalBuffer;

            // 3. The raycast origin is at the enemy's center X, but at its feet Y.
            Vector2 raycastOrigin = new Vector2(transform.position.x, feetY);

            // --- IMPORTANT: The TARGET Y-position for the patrol points remains the enemy's pivot Y.
            // The enemy's Rigidbody moves along transform.position.y, not its feet.
            float targetYPosition = transform.position.y;
            
            // --- The rest of the logic uses this corrected raycastOrigin ---

            float colliderExtentsX = enemyCollider.bounds.extents.x;
            float buffer = 0.05f; // Small horizontal buffer from the wall

            Vector3 leftBoundPoint, rightBoundPoint;

            // Detect Left Bound (using the new 'raycastOrigin')
            RaycastHit2D hitLeft = Physics2D.Raycast(raycastOrigin, Vector2.left, patrolBoundDetectionDistance, whatIsPatrolBoundary);
            leftBoundPoint = hitLeft.collider != null
                ? new Vector3(hitLeft.point.x + colliderExtentsX + buffer, targetYPosition, 0)
                : new Vector3(transform.position.x - patrolBoundDetectionDistance, targetYPosition, 0);

            // Detect Right Bound (using the new 'raycastOrigin')
            RaycastHit2D hitRight = Physics2D.Raycast(raycastOrigin, Vector2.right, patrolBoundDetectionDistance, whatIsPatrolBoundary);
            rightBoundPoint = hitRight.collider != null
                ? new Vector3(hitRight.point.x - colliderExtentsX - buffer, targetYPosition, 0)
                : new Vector3(transform.position.x + patrolBoundDetectionDistance, targetYPosition, 0);

            // Sanity check
            if (leftBoundPoint.x >= rightBoundPoint.x)
            {
                Debug.LogError($"{gameObject.name}: Dynamic patrol points invalid. Defaulting to home position.");
                leftBoundPoint = new Vector3(homePosition.x - 2f, targetYPosition, 0);
                rightBoundPoint = new Vector3(homePosition.x + 2f, targetYPosition, 0);
            }

            patrolPoints = new Vector3[2] { leftBoundPoint, rightBoundPoint };
            
            // Set initial target based on which point is further away
            float distToLeft = Mathf.Abs(transform.position.x - leftBoundPoint.x);
            float distToRight = Mathf.Abs(transform.position.x - rightBoundPoint.x);
            currentPatrolIndex = (distToRight > distToLeft) ? 1 : 0; // 1 is right, 0 is left

            Debug.Log($"{gameObject.name} Dynamic Patrol Initialized. Target: patrolPoints[{currentPatrolIndex}]");
        }

        
        protected void Update()
        {
            if(!CanMove)
            {
                return;
            }

            if(StateMachine != null && player != null)
            {
                StateMachine.Update(this);
            }
        }

        protected void FixedUpdate()
        {
            if(!CanMove || isStunned)
            {
                return;
            }

            if(StateMachine != null && player != null)
            {
                StateMachine.FixedUpdate(this);
            }
        }
        
        protected override void OnDeath()
        {
            DropSuit();
            if(!hasShownSuitTutorial && GSManager.Instance.tutorialsEnabled)
            {
                hasShownSuitTutorial = true;

                TutorialPanelController tutorialPanel = FindObjectOfType<TutorialPanelController>();
                if(tutorialPanel != null)
                {
                    tutorialPanel.ShowMessage(
                        "you acquired a suit! try <color=#00FFFF><b>SHIFT</b></color> and <color=#00FFFF><b>Q</b></color> to use your special abilities.",
                        3f);
                }
            }
            StateMachine = null;
            rb.velocity = Vector2.zero;
            if(enemyCollider) enemyCollider.enabled = false;
            base.OnDeath();
        }

        private void DropSuit()
        {
            if(suitDrop == null)
            {
                Debug.LogWarning($"No suit assigned to drop for {gameObject.name}.", this);
                return;
            }

            GameObject pickup = suitDrop.suitPrefab != null
                                    ? Instantiate(suitDrop.suitPrefab, transform.position, Quaternion.identity)
                                    : CreateDefaultPickup();

            pickup.tag = "Pickup";

            SuitPickup suitPickup = pickup.GetComponent<SuitPickup>();
            if(suitPickup == null)
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
        // In EnemyController.cs


        #if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            // --- PART 1: DYNAMIC PATROL VISUALIZATION ---
            if (useDynamicTwoPointPatrol)
            {
                // Make sure the collider is available, even in Edit Mode.
                if (enemyCollider == null) enemyCollider = GetComponent<Collider2D>();
                if (enemyCollider == null) return; // Exit if no collider is found.

                // --- Replicate the exact raycast logic from InitializeDynamicPatrolPoints ---
                const float verticalBuffer = 0.1f;
                float feetY = enemyCollider.bounds.min.y + verticalBuffer;
                Vector2 raycastOrigin = new Vector2(transform.position.x, feetY);

                // --- Visualize the Left Raycast ---
                RaycastHit2D hitLeft = Physics2D.Raycast(raycastOrigin, Vector2.left, patrolBoundDetectionDistance, whatIsPatrolBoundary);
                if (hitLeft.collider != null)
                {
                    // Draw a GREEN line from the origin to the hit point.
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(raycastOrigin, hitLeft.point);
                    // Draw a small cube at the hit point to make it obvious.
                    Gizmos.DrawWireCube(hitLeft.point, Vector3.one * 0.2f);
                }
                else
                {
                    // Draw a RED line showing the full distance of the raycast.
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(raycastOrigin, raycastOrigin + Vector2.left * patrolBoundDetectionDistance);
                }

                // --- Visualize the Right Raycast ---
                RaycastHit2D hitRight = Physics2D.Raycast(raycastOrigin, Vector2.right, patrolBoundDetectionDistance, whatIsPatrolBoundary);
                if (hitRight.collider != null)
                {
                    // Draw a GREEN line for a successful hit.
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(raycastOrigin, hitRight.point);
                    Gizmos.DrawWireCube(hitRight.point, Vector3.one * 0.2f);
                }
                else
                {
                    // Draw a RED line for a missed hit.
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(raycastOrigin, raycastOrigin + Vector2.right * patrolBoundDetectionDistance);
                }
            }

            // --- PART 2: ACTUAL PATROL PATH VISUALIZATION ---
            // This part draws the path the AI is *actually* using.
            // It works for both dynamic and manually set patrol points.
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                Gizmos.color = Color.yellow;
                Vector3 previousPoint = patrolPoints[patrolPoints.Length - 1];

                foreach (var point in patrolPoints)
                {
                    Gizmos.DrawSphere(point, 0.3f);
                    Gizmos.DrawLine(previousPoint, point);
                    previousPoint = point;
                }

                // For a non-looping path (like our 2-point dynamic patrol),
                // we can skip drawing the line from the last to the first point.
                // The above loop already handles this correctly for 2 points.
            }
        }
        #endif

    }
    
}