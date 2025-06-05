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
            if (rb == null)
            {
                Debug.LogError($"{gameObject.name}: Rigidbody2D not found. Dynamic patrol point initialization failed.", this);
                return;
            }

            Vector2 raycastOrigin = transform.position;
            Vector2 initialYPosition = new Vector2(0, transform.position.y-1); // We only care about the Y
            
            // Ensure enemyCollider is available for calculating offsets
            float colliderExtentsX = 0.1f; // Default small offset if no collider
            if (enemyCollider != null) {
                colliderExtentsX = enemyCollider.bounds.extents.x;
            } else {
                Debug.LogWarning($"{gameObject.name}: EnemyCollider not found for patrol bound offset calculation. Using default small offset.", this);
            }
            float buffer = 0.05f; // Small buffer from the wall

            Vector3 leftBoundPoint;
            Vector3 rightBoundPoint;

            // Detect Left Bound
            RaycastHit2D hitLeft = Physics2D.Raycast(raycastOrigin, Vector2.left, patrolBoundDetectionDistance, whatIsPatrolBoundary);
            if (hitLeft.collider != null)
            {
                leftBoundPoint = new Vector3(hitLeft.point.x + colliderExtentsX + buffer, initialYPosition.y, transform.position.z);
            }
            else
            {
                leftBoundPoint = new Vector3(raycastOrigin.x - (patrolBoundDetectionDistance * 0.5f), initialYPosition.y, transform.position.z);
                Debug.LogWarning($"{gameObject.name}: No left patrol bound detected. Using fallback distance for patrolPoints[0].", this);
            }

            // Detect Right Bound
            RaycastHit2D hitRight = Physics2D.Raycast(raycastOrigin, Vector2.right, patrolBoundDetectionDistance, whatIsPatrolBoundary);
            if (hitRight.collider != null)
            {
                rightBoundPoint = new Vector3(hitRight.point.x - colliderExtentsX - buffer, initialYPosition.y, transform.position.z);
            }
            else
            {
                rightBoundPoint = new Vector3(raycastOrigin.x + (patrolBoundDetectionDistance * 0.5f), initialYPosition.y, transform.position.z);
                Debug.LogWarning($"{gameObject.name}: No right patrol bound detected. Using fallback distance for patrolPoints[1].", this);
            }

            // Sanity check: ensure left is to the left of right
            if (leftBoundPoint.x >= rightBoundPoint.x)
            {
                Debug.LogError($"{gameObject.name}: Dynamic patrol points are invalid (Left: {leftBoundPoint.x}, Right: {rightBoundPoint.x}). Forcing small default range around home: {homePosition}.", this);
                leftBoundPoint = new Vector3(homePosition.x - 1f, initialYPosition.y, transform.position.z);
                rightBoundPoint = new Vector3(homePosition.x + 1f, initialYPosition.y, transform.position.z);
            }

            // Force patrolPoints to be exactly 2 for this dynamic patrol.
            // This ensures the Duri/IraController's Patrol method with `... % patrolPoints.Length` correctly becomes `... % 2`.
            patrolPoints = new Vector3[2];
            patrolPoints[0] = leftBoundPoint;
            patrolPoints[1] = rightBoundPoint;

            // Determine initial patrol direction and index (e.g., move towards the furthest bound or a default)
            // Or simply start by moving towards patrolPoints[1] (right)
            float distToLeft = Mathf.Abs(transform.position.x - patrolPoints[0].x);
            float distToRight = Mathf.Abs(transform.position.x - patrolPoints[1].x);

            // Start moving towards the point we are NOT closest to, or default to point 1 (right)
            if (distToLeft < distToRight && distToLeft < waypointArrivalThreshold * 1.5f) { // If very close to left, target right
                 currentPatrolIndex = 1;
            } else if (distToRight < distToLeft && distToRight < waypointArrivalThreshold * 1.5f) { // If very close to right, target left
                 currentPatrolIndex = 0; // Will effectively move towards left
            } else {
                // Default: aim for patrolPoints[1] (right)
                currentPatrolIndex = 1; // This means the first movement will be towards patrolPoints[1]
                                        // If already at point 1, it will switch to point 0.
                                        // Let's ensure it starts moving, so if it is to pick point 1, set it to 0 and it will move to 1
            }
            
            // To ensure it starts moving towards a point rather than potentially being at it:
            // If we want to start by moving right (towards patrolPoints[1]):
            currentPatrolIndex = 0; // Set index to 0, it will target patrolPoints[0] then switch to patrolPoints[1]
                                    // Or more explicitly if patrolPoints[0] is left and patrolPoints[1] is right:
                                    // To start moving towards right (patrolPoints[1]), the *current* target should be patrolPoints[1].
                                    // The Patrol() logic is: currentTargetPoint = patrolPoints[currentPatrolIndex];
                                    // If it reaches, it increments.
                                    // So, if we want to move to patrolPoints[1] first, set currentPatrolIndex = 1.
                                    // If already near patrolPoints[1], it will then switch to 0.

            // Let's set it to target the one further away, or the right one by default.
            if (transform.position.x < (leftBoundPoint.x + rightBoundPoint.x) / 2) {
                currentPatrolIndex = 1; // Closer to left, target right
            } else {
                currentPatrolIndex = 0; // Closer to right, target left
            }


            Debug.Log($"{gameObject.name} Dynamic Patrol Initialized: patrolPoints[0]={patrolPoints[0]}, patrolPoints[1]={patrolPoints[1]}. Initial index: {currentPatrolIndex}", this);
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
                        "You acquired a Suit! Try Shift and Q to use your special abilities.",
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
        
        #if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
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

           
           
        }
#endif

    }
    
}