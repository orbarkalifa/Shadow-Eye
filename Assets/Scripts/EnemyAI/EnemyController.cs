using EnemyAI;
using Suits;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : Character
{
    [Header("Basic Settings")]
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    public LayerMask playerLayer;
    public Transform player;
    public Rigidbody2D rb;

    // Track last attack time for cooldown logic
    public float LastAttackTime { get; set; }
    public float AttackCooldown => attackCooldown;

    [Header("States (ScriptableObjects)")]
    [Tooltip("The state we start in at runtime.")]
    public EnemyStateSO startingState;

    // If you have direct references to these states, you can assign them here:
    // public EnemyStateSO idleState;
    // public EnemyStateSO chaseState;
    // public EnemyStateSO patrolState;
    // public EnemyStateSO attackState;
    // or simply define them in the Inspector and cross-link them as needed.

    [Header("Patrol Points (Optional)")]
    public Transform[] patrolPoints;
    [HideInInspector] public int currentPatrolIndex = 0;

    [Header("Suit Drop")]
    [SerializeField] private Suit suitDrop;

    public EnemyStateMachine StateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Create the state machine
        StateMachine = new EnemyStateMachine();
        // Initialize with the chosen starting state
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

    /// <summary>
    /// Flip the enemy horizontally based on xDirection.
    /// </summary>
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
        Debug.Log($"{gameObject.name} died.");
        dropSuit();
        base.OnDeath();
        // Optionally, remove or switch to a "DeadStateSO" if you like
    }

    private void dropSuit()
    {
        if (suitDrop != null)
        {
            Debug.Log($"Dropping suit: {suitDrop.suitName}"); 
            GameObject pickup = new GameObject($"{suitDrop.suitName} Pickup");
            pickup.transform.position = transform.position;

            SpriteRenderer spriteRenderer = pickup.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = suitDrop.suitSprite;

            CircleCollider2D collider = pickup.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            pickup.AddComponent<SuitPickup>().Initialize(suitDrop);
        }
        else
        {
            Debug.LogWarning("No suit assigned to drop.");
        }
    }
}
