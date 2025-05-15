using GameStateManagement;
using Suits;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyAI
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : Enemy 
    {
        [Header("Basic Settings")]
        public float attackRange = 5f;
        public float attackCooldown = 2f;
        public Collider2D enemyCollider;
        private static bool hasShownSuitTutorial;

        [HideInInspector] public float lastAttackTime = -Mathf.Infinity;
        public EnemyStateSO startingState;

        public bool canFlee = true;
            
        [Header("Patrol Points")]
        public Vector3[] patrolPoints;
        [HideInInspector] public int currentPatrolIndex; 

        [Header("Suit Drop")]
        [SerializeField] private Suit suitDrop;
        
        public EnemyStateMachine StateMachine { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (enemyCollider == null) enemyCollider = GetComponent<Collider2D>();
            StateMachine = new EnemyStateMachine();
            hasShownSuitTutorial = false;
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
            if(!CanMove) return;
            if (StateMachine != null && player != null)
            {
                StateMachine.Update(this);
            }
        }

        private void FixedUpdate()
        {
            if(!CanMove) return;
            if (StateMachine != null && player != null)
            {
                StateMachine.FixedUpdate(this);
            }
        }

        public void TriggerAttackDamage()
        {
            float recoilDirection;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayerMask);
            foreach (var hitCollider in hitColliders)
            {
                MainCharacter playerController = hitCollider.GetComponent<MainCharacter>();
                if (playerController != null && !playerController.IsInvincible)
                {
                    recoilDirection = GetRecoilDirection(playerController.transform);
                    playerController.TakeDamage(1,recoilDirection);
                    break;
                }
            }
        }
        

        protected override void OnDeath()
        {
            DropSuit();
            if (!hasShownSuitTutorial && GSManager.Instance.tutorialsEnabled)
            {
                hasShownSuitTutorial = true;

                TutorialPanelController tutorialPanel = FindObjectOfType<TutorialPanelController>();
                if (tutorialPanel != null)
                {
                    tutorialPanel.ShowMessage("You acquired a Suit! Try Shift and Q to use your special abilities.", 3f);
                }
            }
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

           
           
        }
#endif
    }
}