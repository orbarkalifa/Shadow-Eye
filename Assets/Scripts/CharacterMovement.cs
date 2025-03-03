using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private static readonly int sr_IsRunning = Animator.StringToHash("isRunning");
    private static readonly int sr_IsJumping = Animator.StringToHash("Jumping");
    private Animator Animator;
    [Header("Movement Settings")]
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float JumpForce = 10f;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private float extraHeight = 3.3f;
    private Rigidbody2D Rb;
    private bool IsFacingRight = true;
    private float HorizontalInput;
    private bool IsDashing = false;
    private bool canDash = false;
    [SerializeField] private float DashSpeed = 35f;
    private const float DashDelay = 0.2f;
    private float originalGravity = 9.5f;    // Store to restore after dash


    protected void Awake()
    {
        Animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        if (!Rb)
            Debug.LogError("Rigidbody2D is missing!");
    }
    
    public void SetHorizontalInput(Vector2 value)
    {
        HorizontalInput = value.x;
    }
    
    public void Move()
    {
        Animator.SetBool(sr_IsRunning, HorizontalInput != 0);
        if (!IsDashing)
            Rb.velocity = new Vector2(HorizontalInput * MoveSpeed, Rb.velocity.y);

        if ((HorizontalInput > 0 && !IsFacingRight) || (HorizontalInput < 0 && IsFacingRight))
            flip();

        UpdateGroundedState();
        HandleFalling();       
    }

    private void UpdateGroundedState()
    {
        Animator.SetBool(sr_IsJumping, !isGrounded());
    }

    private void HandleFalling()
    {
        if (!isGrounded() && Rb.velocity.y < 0)
        {
            Animator.SetBool(sr_IsJumping, true);
        }
    }

    private void flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool isGrounded()
    {
        Vector2 position = transform.position;
        Vector2 boxSize = new Vector2(3.5f, 1f); // Adjust based on character collider size
        Collider2D collider = Physics2D.OverlapBox(position + Vector2.down * extraHeight, boxSize, 0, GroundLayer);
        return collider != null;
    }

    public void Dash()
    {
        if (!IsDashing) // Prevent overlapping dashes
        {
            StartCoroutine(dashWithDelay());
        }
    }

    /*private IEnumerator dashWithDelay()
    {
        IsDashing = true;
        float dashDirection = IsFacingRight ? 1f : -1f;
        Rb.velocity = new Vector2(dashDirection * DashSpeed, Rb.velocity.y);
        yield return new WaitForSeconds(DashDelay);
        IsDashing = false;
    }*/
    private IEnumerator dashWithDelay()
    {
        // Mark dash state
        IsDashing = true;
        canDash = false;
        
        // Temporarily remove gravity so player doesn't fall
        Rb.gravityScale = 0f;

        // Optionally clear current velocity for consistent dash start
        Rb.velocity = Vector2.zero;

        // Determine dash direction
        float dashDirection = IsFacingRight ? 1f : -1f;
        
        // Apply dash velocity
        Rb.velocity = new Vector2(dashDirection * DashSpeed, 0f);

        // Wait for dash duration
        yield return new WaitForSeconds(DashDelay);

        // End dash; restore gravity
        Rb.gravityScale = originalGravity;
        IsDashing = false;

        // Wait out the dash cooldown
        yield return new WaitForSeconds(DashDelay);
        canDash = true;
    }
    
    private void OnDrawGizmos()
    {
        if (GroundLayer != 0)
        {
            Vector2 position = transform.position;
            Vector2 boxSize = new Vector2(3.5f, 1f); 
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(position + Vector2.down*extraHeight, boxSize);
        }
    }
    public void Jump() 
    {
        if (!isGrounded()) return;
        Animator.SetBool(sr_IsJumping, true);
        Rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }
    
    public void OnJumpReleased()
    {
        // Check if the button was released and the velocity is upward
        if (Rb.velocity.y > 0)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, 0);
        }
    }

  
}
