using Suits;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyController : Character
{
    [Header("Enemy Settings")]
    [SerializeField] private float m_MoveSpeed = 2f;
    [SerializeField] private float m_AttackRange = 1.5f;
    [SerializeField] private float m_DetectionRange = 5f;
    [SerializeField] private float m_AttackCooldown = 2f;
    [SerializeField] private LayerMask m_PlayerLayer;
    [SerializeField] private Suit m_SuitDrop;

    private Transform m_Player;
    private bool m_IsChasing = false;
    private float m_LastAttackTime;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        m_Player = GameObject.FindGameObjectWithTag("Player").transform; // Assumes the player has a "Player" tag.
    }

    private void Update()
    {
        if (!m_Player) return;

        float distanceToPlayer = Vector2.Distance(transform.position, m_Player.position);

        // Detect player
        if (distanceToPlayer <= m_DetectionRange && !m_IsChasing)
        {
            m_IsChasing = true;
        }
        else if (distanceToPlayer > m_DetectionRange && m_IsChasing)
        {
            m_IsChasing = false;
        }

        // Handle Attack
        if (m_IsChasing && distanceToPlayer <= m_AttackRange && Time.time >= m_LastAttackTime + m_AttackCooldown)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (m_IsChasing && m_Player)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Debug.Log("ENEMY chase");

        Vector2 direction = (m_Player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * m_MoveSpeed, rb.velocity.y);

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
        m_IsChasing = false;
        Debug.Log("ENEMY ATTACK");
        m_LastAttackTime = Time.time;

        // Check if the player is still in range
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, m_AttackRange, m_PlayerLayer);
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
        if (m_SuitDrop != null)
        {
            Debug.Log($"Dropping suit: {m_SuitDrop.m_SuitName}"); 

            // Create a new game object for the suit pickup
            GameObject pickup = new GameObject($"{m_SuitDrop.m_SuitName} Pickup");
            pickup.transform.position = transform.position;

            // Add a sprite renderer for visual representation
            SpriteRenderer spriteRenderer = pickup.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = m_SuitDrop.m_SuitSprite; // Assume m_SuitSprite is a sprite field in Suit ScriptableObject.

            // Add a collider to make it interactable
            CircleCollider2D collider = pickup.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            // Add SuitPickup component and initialize it
            pickup.AddComponent<SuitPickup>().Initialize(m_SuitDrop);
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
