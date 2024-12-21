using UnityEngine;
using UnityEngine.Serialization;

public class MainCharacter : Character
{
    [FormerlySerializedAs("moveSpeed")]
    [Header("Movement Settings")]
    [SerializeField] private float m_MoveSpeed = 5f;
    [FormerlySerializedAs("jumpForce")]
    [SerializeField] private float m_JumpForce = 10f;
    [FormerlySerializedAs("groundLayer")]
    [SerializeField] private LayerMask m_GroundLayer;

    private Rigidbody2D r_Rb;
    private bool m_IsGrounded;
    private bool m_IsFacingRight = false;
    private float m_HorizontalInput;

    [FormerlySerializedAs("projectilePrefab")]
    [Header("Attack Settings")]
    [SerializeField] private GameObject g_ProjectilePrefab;
    [FormerlySerializedAs("t_FirePoint")]
    [FormerlySerializedAs("firePoint")]
    [SerializeField] private Transform m_FirePoint;

    protected override void Awake()
    {
        base.Awake();

        // Assign Rigidbody2D
        r_Rb = GetComponent<Rigidbody2D>();
        if (!r_Rb)
            Debug.LogError("Rigidbody2D is missing!");
    }

    private void Update()
    {
        // Handle inputs
        m_HorizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && m_IsGrounded)
            jump();

        if (Input.GetButtonDown("Fire1"))
            shoot();
    }

    private void FixedUpdate()
    {
        move(m_HorizontalInput);

        // Check if the character is grounded
        m_IsGrounded = checkIfGrounded();
    }

    private void move(float direction)
    {
        // Apply horizontal movement
        r_Rb.velocity = new Vector2(direction * m_MoveSpeed, r_Rb.velocity.y);

        // Flip the sprite based on movement direction
        if ((direction > 0 && !m_IsFacingRight) || (direction < 0 && m_IsFacingRight))
            flip();
    }

    public void EquipWeapon(string weaponName)
    {
        if (m_CurrentWeapon != null)
        {
            Destroy(m_CurrentWeapon); // Remove the old weapon
        }

        // Instantiate the new weapon (assumes weapon prefabs are stored in a manager)
        GameObject newWeapon = WeaponManager.m_Instance.GetWeaponByName(weaponName);
        if (newWeapon != null)
        {
            Debug.Log("Weapon is not null");
            Vector3 position = new Vector3(m_WeaponHolder.position.x, m_WeaponHolder.position.y, -1);
            m_CurrentWeapon = Instantiate(newWeapon, position, m_WeaponHolder.rotation, m_WeaponHolder);
        }
        if (m_CurrentWeapon == null)
            Debug.LogError("m_CurrentWeapon weapon is null");
    }
    
    private void jump()
    {
        r_Rb.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
    }

    private void shoot()
    {
        if (g_ProjectilePrefab && m_FirePoint)
        {
            // Instantiate projectile and shoot in the direction the character is facing
            GameObject projectile = Instantiate(g_ProjectilePrefab, m_FirePoint.position, Quaternion.identity);
            Vector2 shootDirection = m_IsFacingRight ? Vector2.right : Vector2.left;
            projectile.GetComponent<Projectile>().Initialize(shootDirection);
        }
        else
        {
            Debug.LogWarning("Projectile prefab or fire point not assigned.");
        }
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

}