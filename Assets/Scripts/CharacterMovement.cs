using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float m_MoveSpeed = 5f;
    [SerializeField] private float m_JumpForce = 10f;
    [SerializeField] private LayerMask m_GroundLayer;

    private Rigidbody2D r_Rb;
    private bool m_IsFacingRight = false;
    private float m_HorizontalInput;
    protected  void Awake()
    {
        // Assign Rigidbody2D
        r_Rb = GetComponent<Rigidbody2D>();
        if (!r_Rb)
            Debug.LogError("Rigidbody2D is missing!");
    }

    public void SetHorizontalInput(Vector2 value)
    {
        m_HorizontalInput = value.x;
    }
    public void Move()
    {
        // Apply horizontal movement
        r_Rb.velocity = new Vector2(m_HorizontalInput * m_MoveSpeed, r_Rb.velocity.y);

        // Flip the sprite based on movement direction
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
        Vector2 boxSize = new Vector2(0.9f, 0.1f); // Adjust based on character collider size
        
        Collider2D collider = Physics2D.OverlapBox(position + Vector2.down * extraHeight, boxSize, 0, m_GroundLayer);
        return collider != null;
    }
    public void Jump()
    { 
        if (!checkIfGrounded()) return;
        r_Rb.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
    }
}
