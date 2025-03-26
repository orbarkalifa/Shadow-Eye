using EnemyAI;
using Suits;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : Character
{
    [Header("Basic Settings")]
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public LayerMask playerLayer;
    public Transform player;
    public Rigidbody2D rb;

    public float lastAttackTime;
    public EnemyStateSO startingState;

    [Header("Patrol Points")]
    public Vector3[] patrolPoints;
    [HideInInspector] public int currentPatrolIndex;
    public float detectionRange = 10f;

    [Header("Suit Drop")]
    [SerializeField] private Suit suitDrop;

    public EnemyStateMachine StateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null)
            {
                Debug.LogError("EnemyController: Player not found! Make sure player has 'Player' tag.");
            }
        }
        StateMachine = new EnemyStateMachine();
        StateMachine.Initialize(this, startingState);
    }
    
    private void Update()
    {
        if (!player) return;
        StateMachine.Update(this);
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate(this);
    }

    public void UpdateFacingDirection(float xDirection)
    {
        if (xDirection > 0 && transform.localScale.x > 0)
        {
            Flip();
        }
        else if (xDirection < 0 && transform.localScale.x < 0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected override void OnDeath()
    {
        DropSuit();
        base.OnDeath();
    }

    private void DropSuit()
    {
        if (suitDrop == null)
        {
            Debug.LogWarning("No suit assigned to drop.");
            return;
        }

        // Use the prefab if provided, otherwise create a default pickup.
        GameObject pickup = suitDrop.suitPrefab != null 
                                ? Instantiate(suitDrop.suitPrefab, transform.position, Quaternion.identity)
                                : CreateDefaultPickup();

        pickup.tag = "Pickup";

        // Ensure it has a SuitPickup component and initialize it.
        SuitPickup suitPickup = pickup.GetComponent<SuitPickup>();
        if (suitPickup == null)
        {
            suitPickup = pickup.AddComponent<SuitPickup>();
        }
        suitPickup.Initialize(suitDrop);
    }

    private GameObject CreateDefaultPickup()
    {
        GameObject pickup = new GameObject($"{suitDrop.suitName} Pickup");
        pickup.transform.position = transform.position;

        SpriteRenderer sr = pickup.AddComponent<SpriteRenderer>();
        sr.sprite = suitDrop.suitSprite;

        CircleCollider2D physicalCollider = pickup.AddComponent<CircleCollider2D>();
        physicalCollider.isTrigger = false;

        CircleCollider2D triggerCollider = pickup.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;

        Rigidbody2D suitRb = pickup.AddComponent<Rigidbody2D>();
        suitRb.freezeRotation = true;

        return pickup;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // --- Draw Patrol Points ---
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.yellow; // Color for patrol points
            Vector3 previousPoint = patrolPoints[patrolPoints.Length - 1]; // Start with the last point for loop connection

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Vector3 currentPoint = patrolPoints[i];

                // Draw a sphere handle at each patrol point
                EditorGUI.BeginChangeCheck(); // Start checking for changes
                // Use a PositionHandle for interactive movement
                Vector3 newPosition = Handles.PositionHandle(currentPoint, Quaternion.identity);
                if (EditorGUI.EndChangeCheck()) // Check if handle was moved
                {
                    Undo.RecordObject(this, "Move Patrol Point"); // Enable Undo functionality
                    patrolPoints[i] = newPosition; // Update patrol point position
                    currentPoint = newPosition; // Use the new position immediately for drawing lines
                }

                // Draw sphere gizmo at the point's position
                Gizmos.DrawSphere(currentPoint, 0.3f);

                // Draw lines connecting patrol points
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint; // Update previous point for the next iteration
            }
        }

        // --- Draw Attack Range ---
        Gizmos.color = Color.red; // Color for the attack range circle
        if (transform != null) // Check if transform exists
        {
            // Draw a wire sphere centered on the enemy's current position
            // with a radius equal to the attackRange.
            // In 2D view, this will appear as a circle.
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

         // --- Draw Detection Range (Optional but consistent) ---
         Gizmos.color = Color.cyan; // Choose a different color for detection
         if (transform != null)
         {
             Gizmos.DrawWireSphere(transform.position, detectionRange);
         }
    }
#endif
}
