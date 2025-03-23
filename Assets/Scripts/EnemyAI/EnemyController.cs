using EnemyAI;
using Suits;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : Character
{
    public Animator animator;
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Basic Settings")]
    public float moveSpeed = 2f;
    public float attackRange = 3f;
    [SerializeField] private float attackCooldown = 2f;
    public LayerMask playerLayer;
    public Transform player;
    public Rigidbody2D rb;

    public float LastAttackTime { get; set; }
    public float AttackCooldown => attackCooldown;
    public EnemyStateSO startingState;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;
    [HideInInspector] public int currentPatrolIndex = 0;

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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (StateMachine != null && StateMachine.CurrentState is ChaseStateSO chaseState) // Check if in chase state and get attackRange from ChaseStateSO
        {
            Gizmos.DrawWireSphere(transform.position, chaseState.attackRange);
        } else {
            // Fallback, use EnemyController's attackRange (less accurate for ChaseState visualization, but still helpful)
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
#endif
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
        dropSuit();
        base.OnDeath();
    }

    private void dropSuit()
    {
        if (suitDrop != null)
        {
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
