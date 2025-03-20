using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterMovement : MonoBehaviour
{
    private static readonly int isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int isJumpingHash = Animator.StringToHash("Jumping");
    private static readonly int isWallSlidingHash = Animator.StringToHash("isWallSliding");

    private Animator animator;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private float horizontalInput;
    private bool isDashing;
    
    private int jumpCount;
    private readonly int maxJumpCount = 1; // Change this to 2 if you want to allow double jump
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float jumpForce = 35;
    [SerializeField] private float extraHeight = 1.5f;

    [Header("Dash Settings")]
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashSpeed = 35f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Jump Tuning")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float variableJumpMultiplier = 0.5f;

    [FormerlySerializedAs("GroundLayer")]
    [Header("Wall Jumping")]
    [SerializeField] private LayerMask groundLayer;
    [FormerlySerializedAs("WallLayer")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 20f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1.5f, 1f);
    [SerializeField] private float wallJumpCooldown = 0.2f;

    private float coyoteTimeCounter;
    private bool isWallSliding;
    private bool canWallJump;
    private bool isWallJumping;

    private float lastDashTime = -Mathf.Infinity;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
            Debug.LogError("Rigidbody2D is missing!");
    }

    private void Update()
    {
        animator.SetBool(isRunningHash, horizontalInput != 0);

        if (isGrounded())
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (!isWallJumping)
        {
            handleWallSliding();
        }
        else
        {
            animator.SetBool(isWallSlidingHash, false);
        }

        handleFalling();
    }

    private bool isTouchingWall()
    {
        Vector2 position = transform.position;
        float wallCheckDistance = 1f;
        bool leftCheck = Physics2D.Raycast(position, Vector2.left, wallCheckDistance, wallLayer);
        bool rightCheck = Physics2D.Raycast(position, Vector2.right, wallCheckDistance, wallLayer);
        return leftCheck || rightCheck;
    }

    private void handleWallSliding()
    {
        isWallSliding = isTouchingWall() && !isGrounded() && horizontalInput != 0;
        animator.SetBool(isWallSlidingHash, isWallSliding);

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
                rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            
            if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
                flip();

            updateGroundedState();
        }
    }

    private void updateGroundedState()
    {
        animator.SetBool(isJumpingHash, !isGrounded());
    }

    private void handleFalling()
    {
        if (!isGrounded() && rb.velocity.y < 0)
            animator.SetBool(isJumpingHash, true);
    }

    private void flip()
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
        Collider2D groundDetected = Physics2D.OverlapBox(position + Vector2.down * extraHeight, boxSize, 0f, groundLayer);
    
        if (groundDetected)
            jumpCount = 0;
    
        return groundDetected;
    }
    
    private void OnDrawGizmos()
    {
        if (groundLayer == 0)
            return;
        Vector2 position = transform.position;
        Vector2 boxSize = new Vector2(0.8f, 0.8f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(position + Vector2.down * extraHeight, boxSize);
    }

    public void Dash()
    {
        if(isDashing || !(Time.time >= lastDashTime + dashCooldown))
        {
            return;
        }

        StartCoroutine(dashRoutine());
        lastDashTime = Time.time;
    }

    private IEnumerator dashRoutine()
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
    

    public void Jump() 
    {
        if (isWallSliding)
        {
            wallJump();
        }
        else if ((coyoteTimeCounter > 0f || isGrounded()) && jumpCount < maxJumpCount) 
        {
            animator.SetBool(isJumpingHash, true);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++; 
            coyoteTimeCounter = 0;
        }
    }

    private void wallJump()
    {
        if (!canWallJump)
            return;

        canWallJump = false;

        float direction = isFacingRight ? -1 : 1;
        var jumpDirection = new Vector2(wallJumpDirection.x * direction, wallJumpDirection.y).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);

        flip();

        isWallJumping = true;
        StartCoroutine(wallJumpRoutine());
    }

    private IEnumerator wallJumpRoutine()
    {
        yield return new WaitForSeconds(wallJumpCooldown);
        canWallJump = true;
        isWallJumping = false;
    }
    
    public void OnJumpReleased()
    {
        if (rb.velocity.y > 0)
            rb.velocity += Vector2.up * -jumpForce * variableJumpMultiplier;
    }
    
}
