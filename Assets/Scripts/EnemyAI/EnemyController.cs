using EnemyAI;
using Suits;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : Character
{
    public Animator animator;
    [Header("Basic Settings")]
    public float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    public LayerMask playerLayer;
    public Transform player;
    public Rigidbody2D rb;

    public float LastAttackTime { get; set; }
    public float AttackCooldown => attackCooldown;
    public EnemyStateSO startingState;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;
    [HideInInspector] public int currentPatrolIndex;

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
        Gizmos.DrawWireSphere(transform.position, attackRange);
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
}
