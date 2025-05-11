using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CharacterMovement : MonoBehaviour
{
    private static readonly int isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int isJumpingHash = Animator.StringToHash("Jumping");
    private static readonly int isWallSlidingHash = Animator.StringToHash("isWallSliding");
    [SerializeField] private float recoilForce = 50;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private float horizontalInput;
    private bool isDashing;
    private bool canMove = true;
    private int jumpCount;
    private readonly int maxJumpCount = 1; // Change this to 2 if you want to allow double jump
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float jumpForce = 35;
    [SerializeField] private float extraHeight = 1.5f;

    [Header("Dash Settings")]
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashSpeed = 35f;

    [Header("Jump Tuning")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float variableJumpMultiplier = 0.5f;

    [Header("Wall Jumping")]
    public bool canWallGrab;
    [SerializeField] private LayerMask groundLayer;
    [FormerlySerializedAs("WallLayer")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 20f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1.5f, 1f);
    [SerializeField] private float wallJumpCooldown = 0.2f;

    [Header("Enemy Layer")]
    [SerializeField] private LayerMask enemyLayer; // Assign the Enemy Layer in the Inspector

    private float coyoteTimeCounter;
    private bool isWallSliding;
    private bool canWallJump;
    private bool isWallJumping;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
            Debug.LogError("Rigidbody2D is missing!");
    }

    private void Update()
    {
        animator.SetBool(isRunningHash, horizontalInput != 0 && IsGrounded());
        if (IsGrounded())
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (!isWallJumping)
        {
            HandleWallSliding();
        }
        else
        {
            animator.SetBool(isWallSlidingHash, false);
        }

        HandleFalling();
    }
    
    private bool IsTouchingWall()
    {
        Vector2 position = transform.position;
        float wallCheckDistance = 1f;
    
        bool wallOnRight = Physics2D.Raycast(position, Vector2.right, wallCheckDistance, wallLayer);
        bool wallOnLeft = Physics2D.Raycast(position, Vector2.left, wallCheckDistance, wallLayer);
    
        if (wallOnRight && isFacingRight)
            return true;
        if (wallOnLeft && !isFacingRight)
            return true;
    
        return false;
    }

    private void HandleWallSliding()
    {
        isWallSliding = IsTouchingWall() && !IsGrounded() && canWallGrab;
        animator.SetBool(isWallSlidingHash, isWallSliding);
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            canWallJump = true;
        }
    }
    
    public void Move()
    {
        if(canMove)
        {
            if(!isWallJumping)
            {
                if(!isDashing)
                    rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

                if((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
                    Flip();

                UpdateGroundedState();
            }
        }
    }

    public void SetHorizontalInput(float value)
    {
        horizontalInput = value;
    }

    private void UpdateGroundedState()
    {
        animator.SetBool(isJumpingHash, !IsGrounded() && !isWallSliding); // activate or deactivate jump animation
    }

    private void HandleFalling()
    {
        if (!IsGrounded() && rb.velocity.y < 0 && !isWallSliding)
            animator.SetBool(isJumpingHash, true);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool IsGrounded()
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
        Vector2 position = transform.position-new Vector3(0.1f,0,0);
        Vector2 boxSize = new Vector2(0.8f, 0.8f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(position + Vector2.down * extraHeight, boxSize);
    }

    public void Dash()
    {
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        BoxCollider2D myCollider = GetComponent<BoxCollider2D>();
        int playerLayer = gameObject.layer; 
        int layerToIgnoreOnDash = LayerMask.NameToLayer("Enemy") | LayerMask.NameToLayer("Deadly") ; 

        if (layerToIgnoreOnDash == -1)
        {
            Debug.LogWarning("Layer 'Enemy' or 'Deadly' not found. Falling back to disabling collider completely for dash.");
            myCollider.enabled = false; 
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerLayer, layerToIgnoreOnDash, true);
        }

        isDashing = true;
        animator.CrossFadeInFixedTime("Dash", 0.05f);

        float storedGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        float dashDirection = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        if (layerToIgnoreOnDash != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, layerToIgnoreOnDash, false);
        }
        else
        {
            myCollider.enabled = true;
        }

        rb.gravityScale = storedGravity;
        isDashing = false;
    }


    public void Jump()
    {
        if (isWallSliding)
        {
            WallJump();
        }
        else if ((coyoteTimeCounter > 0f && rb.velocity.y<0 || IsGrounded()) && jumpCount < maxJumpCount)
        {
            jumpCount++;
            coyoteTimeCounter = 0;
            animator.SetBool(isJumpingHash, true);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            
        }
    }

    private void WallJump()
    {
        if (!canWallJump)
            return;

        animator.SetBool(isJumpingHash, true);
        
        canWallJump = false;

        float direction = isFacingRight ? -1 : 1;
        var jumpDirection = new Vector2(wallJumpDirection.x * direction, wallJumpDirection.y).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);

        Flip();

        isWallJumping = true;
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
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
    public void AddRecoil(float recoilDirection)
    {
        StartCoroutine(RecoilCoroutine(recoilDirection));
    }

    private IEnumerator RecoilCoroutine(float recoilDirection)
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        Debug.Log($"Applying {recoilDirection} * {recoilForce}");
        rb.AddForce(new Vector2(recoilDirection * recoilForce, 0), ForceMode2D.Impulse);
        // Wait for a short duration to allow the recoil to take effect.
        yield return new WaitForSeconds(0.2f);
        canMove = true;
    }
}