using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float m_MoveSpeed = 5f;
    [SerializeField] private float m_JumpForce = 10f;
    [SerializeField] private LayerMask m_GroundLayer;

    private Rigidbody2D rb;
    private bool m_IsFacingRight = false;
    private float m_HorizontalInput;
    private bool m_IsDashing = false;
    [SerializeField]private float m_DashSpeed = 1000f;
    private float m_DashDelay = 0.1f;

    protected  void Awake()
    {
        // Assign Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
            Debug.LogError("Rigidbody2D is missing!");
    }

    public void SetHorizontalInput(Vector2 value)
    {
        m_HorizontalInput = value.x;
    }
    public void Move()
    {
        if(!m_IsDashing)
            rb.velocity = new Vector2(m_HorizontalInput * m_MoveSpeed, rb.velocity.y);

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

    private bool checkIfGrounded()
    {
        float extraHeight = 0.7f;
        Vector2 position = transform.position;
        Vector2 boxSize = new Vector2(0.3f, 0.3f); // Adjust based on character collider size
        
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
        rb.velocity = new Vector2(dashDirection * m_DashSpeed, rb.velocity.y);

        yield return new WaitForSeconds(m_DashDelay);

        m_IsDashing = false;
    }
    
    private void OnDrawGizmos()
    {
        if (m_GroundLayer != 0)
        {
            float extraHeight = 0.7f;
            Vector2 position = transform.position;
            Vector2 boxSize = new Vector2(0.3f, 0.3f); // Adjust based on character collider size

            Gizmos.color = Color.yellow; // Color for the ground check box
            Gizmos.DrawWireCube(position + Vector2.down*extraHeight, boxSize);
        }
    }
    public void Jump()
    {
        
        if (!checkIfGrounded()) return;
        rb.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
    }
    
    public void OnJumpReleased()
    {
        // Check if the button was released and the velocity is upward
        if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }
}
