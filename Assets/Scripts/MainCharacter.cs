using UnityEngine;

public class MainCharacter : Character
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = false;
    private float horizontalInput;

    [Header("Attack Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    protected override void Awake()
    {
        base.Awake();

        // Assign Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
            Debug.LogError("Rigidbody2D is missing!");
    }

    private void Update()
    {
        // Handle inputs
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        if (Input.GetButtonDown("Fire1"))
            Shoot();
    }

    private void FixedUpdate()
    {
        Move(horizontalInput);

        // Check if the character is grounded
        isGrounded = CheckIfGrounded();
    }

    private void Move(float direction)
    {
        // Apply horizontal movement
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Flip the sprite based on movement direction
        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
            Flip();
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Shoot()
    {
        if (projectilePrefab && firePoint)
        {
            // Instantiate projectile and shoot in the direction the character is facing
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;
            projectile.GetComponent<Projectile>().Initialize(shootDirection);
        }
        else
        {
            Debug.LogWarning("Projectile prefab or fire point not assigned.");
        }
    }

    private void Flip()
    {
        // Flip the character sprite
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool CheckIfGrounded()
    {
        float extraHeight = 0.7f;
        Vector2 position = transform.position;
        Vector2 boxSize = new Vector2(0.9f, 0.1f); // Adjust based on character collider size
    
        // Draw the box for visualization
        Debug.DrawLine(position + Vector2.down * extraHeight + Vector2.left * boxSize.x / 2,
            position + Vector2.down * extraHeight + Vector2.right * boxSize.x / 2, 
            Color.red, 0.1f);
    
        Collider2D collider = Physics2D.OverlapBox(position + Vector2.down * extraHeight, boxSize, 0, groundLayer);
        return collider != null;
    }

}