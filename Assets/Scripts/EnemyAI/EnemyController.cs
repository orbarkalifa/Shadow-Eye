using EnemyAI;
using Suits;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : Character
{
    public Animator animator;
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
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                // Draw a sphere at each patrol point
                Gizmos.DrawSphere(patrolPoints[i], 0.3f);

                // Draw lines connecting patrol points
                if (i > 0)
                {
                    Gizmos.DrawLine(patrolPoints[i - 1], patrolPoints[i]);
                }
                else if (patrolPoints.Length > 1)
                {
                    Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1], patrolPoints[0]); // Loop back to start
                }

                // Add handles to move patrol points in the editor
                EditorGUI.BeginChangeCheck(); // Start checking for changes
                Vector3 newPosition = Handles.PositionHandle(patrolPoints[i], Quaternion.identity); // Create an interactive handle
                if (EditorGUI.EndChangeCheck()) // Check if handle was moved
                {
                    Undo.RecordObject(this, "Move Patrol Point"); // Enable Undo functionality
                    patrolPoints[i] = newPosition; // Update patrol point position
                }
            }
        }
    }
#endif
}
