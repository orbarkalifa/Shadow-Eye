using Suits;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyController : Character
{
    [Header("Enemy Settings")]
    [SerializeField] private float MoveSpeed = 2f;
    [SerializeField] private float AttackRange = 1.5f;
    [SerializeField] private float DetectionRange = 5f;
    [SerializeField] private float AttackCooldown = 2f;
    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private Suit SuitDrop;

    private Transform Player;
    private bool IsChasing = false;
    private float LastAttackTime;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        Player = GameObject.FindGameObjectWithTag("Player").transform; // Assumes the player has a "Player" tag.
    }

    private void Update()
    {
        if (!Player) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        // Detect player
        if (distanceToPlayer <= DetectionRange && !IsChasing)
        {
            IsChasing = true;
        }
        else if (distanceToPlayer > DetectionRange && IsChasing)
        {
            IsChasing = false;
        }

        // Handle Attack
        if (IsChasing && distanceToPlayer <= AttackRange && Time.time >= LastAttackTime + AttackCooldown)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (IsChasing && Player)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Debug.Log("ENEMY chase");

        Vector2 direction = (Player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * MoveSpeed, rb.velocity.y);

        // Flip sprite based on direction
        if (direction.x < 0 && transform.localScale.x < 0 || direction.x > 0 && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void Attack()
    {
        IsChasing = false;
        Debug.Log("ENEMY ATTACK");
        LastAttackTime = Time.time;

        // Check if the player is still in range
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, AttackRange, PlayerLayer);
        if (playerCollider)
        {
            MainCharacter playerController = playerCollider.GetComponent<MainCharacter>();
            if (playerController)
            {
                playerController.TakeDamage(1); // Example damage value
            }
        }
    }

    protected override void OnDeath()
    {
        Debug.Log($"{gameObject.name} died."); 
        dropSuit();
        base.OnDeath();
    }

    
    private void dropSuit()
    {
        if (SuitDrop != null)
        {
            Debug.Log($"Dropping suit: {SuitDrop.m_SuitName}"); 

            // Create a new game object for the suit pickup
            GameObject pickup = new GameObject($"{SuitDrop.m_SuitName} Pickup");
            pickup.transform.position = transform.position;

            // Add a sprite renderer for visual representation
            SpriteRenderer spriteRenderer = pickup.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = SuitDrop.m_SuitSprite; // Assume m_SuitSprite is a sprite field in Suit ScriptableObject.

            // Add a collider to make it interactable
            CircleCollider2D collider = pickup.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            // Add SuitPickup component and initialize it
            pickup.AddComponent<SuitPickup>().Initialize(SuitDrop);
        }
        else
        {
            Debug.LogWarning("No suit assigned to drop.");
        }
    }
    
    
    /*private void dropSuit()
    {
        if (m_Suit)
        {
            Debug.Log($"Dropping weapon: {m_Suit.name}");
            Instantiate(m_Suit, new Vector3(transform.position.x,transform.position.y,-1), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("m_CurrentWeapon is null, no weapon to drop.");
        }
    }*/
}
