using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private static readonly int sr_IsRunning = Animator.StringToHash("isRunning");
    private static readonly int sr_IsJumping = Animator.StringToHash("Jumping");
    private static readonly int sr_IsWallSliding = Animator.StringToHash("isWallSliding");

    private Animator animator;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private float horizontalInput;
    private bool isDashing;
    
    private int jumpCount = 0;
    private int maxJumpCount = 1; // Change this to 2 if you want to allow double jump


    [Header("Movement Settings")]
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float JumpForce = 10f;
    [SerializeField] private float extraHeight = 1.5f;

    [Header("Dash Settings")]
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashSpeed = 35f;
    [SerializeField] private float dashCooldown = 1f; // Cooldown period between dashes

    [Header("Jump Tuning")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float variableJumpMultiplier = 0.5f;

    [Header("Wall Jumping")]
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private LayerMask WallLayer;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 20f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1.5f, 1f);
    [SerializeField] private float wallJumpCooldown = 0.2f;

    private float coyoteTimeCounter;
    private bool isWallSliding;
    private bool canWallJump = true;
    private bool isWallJumping; // Temporarily disable wall sliding after a wall jump

    private float lastDashTime = -Mathf.Infinity; // Timestamp for dash cooldown

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
            Debug.LogError("Rigidbody2D is missing!");
    }

    private void Update()
    {
        animator.SetBool(sr_IsRunning, horizontalInput != 0);

        // Handle coyote time
        if (isGrounded())
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Only process wall sliding if not in the middle of a wall jump
        if (!isWallJumping)
        {
            HandleWallSliding();
        }
        else
        {
            animator.SetBool(sr_IsWallSliding, false);
        }

        HandleFalling();
    }

    private bool IsTouchingWall()
    {
        Vector2 position = transform.position;
        float wallCheckDistance = 0.5f;
        bool leftCheck = Physics2D.Raycast(position, Vector2.left, wallCheckDistance, WallLayer);
        bool rightCheck = Physics2D.Raycast(position, Vector2.right, wallCheckDistance, WallLayer);
        return leftCheck || rightCheck;
    }

    private void HandleWallSliding()
    {
        isWallSliding = IsTouchingWall() && !isGrounded() && horizontalInput != 0;
        animator.SetBool(sr_IsWallSliding, isWallSliding);

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            canWallJump = true;
        }
    }

    public void SetHorizontalInput(Vector2 value)
    {
        horizontalInput = value.x;
    }
    
    public void Move()
    {
        if(!isWallJumping)
        {
            if (!isDashing)
                rb.velocity = new Vector2(horizontalInput * MoveSpeed, rb.velocity.y);
            
            if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
                Flip();

            UpdateGroundedState();

        }
        
    }

    private void UpdateGroundedState()
    {
        animator.SetBool(sr_IsJumping, !isGrounded());
    }

    private void HandleFalling()
    {
        if (!isGrounded() && rb.velocity.y < 0)
            animator.SetBool(sr_IsJumping, true);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool isGrounded()
    {
        Vector2 position = transform.position;
        Vector2 boxSize = new Vector2(0.8f, 0.8f);
        Collider2D collider = Physics2D.OverlapBox(position + Vector2.down * extraHeight, boxSize, 0f, GroundLayer);
        bool grounded = collider != null;
    
        // Reset jump count when grounded
        if (grounded)
            jumpCount = 0;
    
        return grounded;
    }

    public void Dash()
    {
        if (!isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(DashRoutine());
            lastDashTime = Time.time;
        }
    }

    private IEnumerator DashRoutine()
    {
        BoxCollider2D myCollider = GetComponent<BoxCollider2D>();
        myCollider.enabled = false;
        isDashing = true;
        animator.CrossFadeInFixedTime("Dash", 0.05f);

        float storedGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        float dashDirection = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        myCollider.enabled = true;
        rb.gravityScale = storedGravity;
        isDashing = false;
    }

    private void OnDrawGizmos()
    {
        if (GroundLayer == 0)
            return;
        Vector2 position = transform.position;
        Vector2 boxSize = new Vector2(0.8f, 0.8f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(position + Vector2.down * extraHeight, boxSize);
    }

    public void Jump() 
    {
        if (isWallSliding)
        {
            WallJump();
        }
        else if ((coyoteTimeCounter > 0f || isGrounded()) && jumpCount < maxJumpCount) 
        {
            animator.SetBool(sr_IsJumping, true);
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            jumpCount++; // Increment to prevent additional jumps
            coyoteTimeCounter = 0;
        }
    }

    private void WallJump()
    {
        if (!canWallJump)
            return;

        Debug.Log("Wall jump");
        canWallJump = false;

        float direction = isFacingRight ? -1 : 1;
        Vector2 jumpDirection = new Vector2(wallJumpDirection.x * direction, wallJumpDirection.y).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);

        Flip();

        isWallJumping = true;
        StartCoroutine(WallJumpCooldown());
    }

    private IEnumerator WallJumpCooldown()
    {
        yield return new WaitForSeconds(wallJumpCooldown);
        canWallJump = true;
        isWallJumping = false;
    }
    
    public void OnJumpReleased()
    {
        if (rb.velocity.y > 0)
            rb.velocity += Vector2.up * -JumpForce * variableJumpMultiplier;
    }
}
