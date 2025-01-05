using UnityEngine;

public class EnemyController : Character
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject suit;

    private Transform player;
    private bool isChasing = false;
    private float lastAttackTime;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Assumes the player has a "Player" tag.
    }

    private void Update()
    {
        if (!player) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Detect player
        if (distanceToPlayer <= detectionRange && !isChasing)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange && isChasing)
        {
            isChasing = false;
        }

        // Handle Attack
        if (isChasing && distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (isChasing && player)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Debug.Log("ENEMY chase");

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

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
        isChasing = false;
        Debug.Log("ENEMY ATTACK");
        lastAttackTime = Time.time;

        // Check if the player is still in range
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (playerCollider)
        {
            Debug.Log("first");
            MainCharacter playerController = playerCollider.GetComponent<MainCharacter>();
            if (playerController)
            {
                Debug.Log("second");
                playerController.TakeDamage(1); // Example damage value
            }
        }
    }

    protected override void OnDeath()
    {
        Debug.Log("DIED");
        dropSuit();
        base.OnDeath();
    }

    private void dropSuit()
    {
        if (suit)
        {
            Debug.Log($"Dropping weapon: {suit.name}");
            Instantiate(suit, new Vector3(transform.position.x,transform.position.y,-1), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("m_CurrentWeapon is null, no weapon to drop.");
        }
    }
}
