using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private static readonly int sr_IsRunning = Animator.StringToHash("isRunning");
    private static readonly int sr_IsJumping = Animator.StringToHash("Jumping");
    private Animator m_Animator;
    [Header("Movement Settings")]
    [SerializeField] private float m_MoveSpeed = 5f;
    [SerializeField] private float m_JumpForce = 10f;
    [SerializeField] private LayerMask m_GroundLayer;

    private Rigidbody2D m_Rb;
    private bool m_IsFacingRight = true;
    private float m_HorizontalInput;
    private bool m_IsDashing = false;
    [SerializeField]private float m_DashSpeed = 1000f;
    private float m_DashDelay = 0.1f;

    protected  void Awake()
    {
        m_Animator = GetComponent<Animator>();
        // Assign Rigidbody2D
        m_Rb = GetComponent<Rigidbody2D>();
        if (!m_Rb)
            Debug.LogError("Rigidbody2D is missing!");
    }

    public void SetHorizontalInput(Vector2 value)
    {
        m_HorizontalInput = value.x;
    }
    public void Move()
    {
        m_Animator.SetBool(sr_IsRunning,m_HorizontalInput != 0);
        if(!m_IsDashing)
            m_Rb.velocity = new Vector2(m_HorizontalInput * m_MoveSpeed, m_Rb.velocity.y);

        if ((m_HorizontalInput > 0 && !m_IsFacingRight) || (m_HorizontalInput < 0 && m_IsFacingRight))
            flip();
        
    }
    private void flip()
    {
        // Flip the character sprite
        m_IsFacingRight = !m_IsFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool isGrounded()
    {
        float extraHeight = 1f;
        Vector2 position = transform.position;
        Vector2 boxSize = new Vector2(0.8f, 0.5f); // Adjust based on character collider size
        
        Collider2D collider = Physics2D.OverlapBox(position + Vector2.down * extraHeight, boxSize, 0, m_GroundLayer);
      
        return collider != null;
    }

    public void Dash()
    {
        if (!m_IsDashing) // Prevent overlapping dashes
        {
            StartCoroutine(dashWithDelay());
        }
    }

    private IEnumerator dashWithDelay()
    {
        m_IsDashing = true;

        float dashDirection = m_IsFacingRight ? 1f : -1f;
        m_Rb.velocity = new Vector2(dashDirection * m_DashSpeed, m_Rb.velocity.y);

        yield return new WaitForSeconds(m_DashDelay);

        m_IsDashing = false;
    }
    
    private void OnDrawGizmos()
    {
        if (m_GroundLayer != 0)
        {
            float extraHeight = 0.9f;
            Vector2 position = transform.position;
            Vector2 boxSize = new Vector2(0.8f, 0.3f); // Adjust based on character collider size

            Gizmos.color = Color.yellow; // Color for the ground check box
            Gizmos.DrawWireCube(position + Vector2.down*extraHeight, boxSize);
        }
    }
    public void Jump() 
    {
        
        if (!isGrounded()) return;
        m_Animator.SetBool(sr_IsJumping, true);

        

        m_Rb.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
    }
    
    public void OnJumpReleased()
    {
        // Check if the button was released and the velocity is upward
        if (m_Rb.velocity.y > 0)
        {
            m_Rb.velocity = new Vector2(m_Rb.velocity.x, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            m_Animator.SetBool(sr_IsJumping, false);
        }
    }
}
