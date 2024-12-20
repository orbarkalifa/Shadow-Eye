using UnityEngine;

public class MainCharacter : Character
{
    [SerializeField] private Rigidbody2D m_Rigidbody;

    [Header("Movement Settings")]
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private float m_JumpForce = 5f;

    private bool m_IsGrounded;

    protected override void Awake()
    {
        base.Awake();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        if (!m_Rigidbody)
            Debug.LogError("MainCharacter: Rigidbody2D missing!");
    }

    private void Update()
    {
        handleMovement();
        HandleJump();

        if (Input.GetButtonDown("Fire1"))
            shootProjectile();
    }

    private void handleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Flip character sprite based on direction
        if (horizontalInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(-horizontalInput), 1, 1);

        // Apply movement
        m_Rigidbody.velocity = new Vector2(horizontalInput * m_Speed, m_Rigidbody.velocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && m_IsGrounded)
        {
            m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
            m_IsGrounded = false;
        }
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
            m_CurrentWeapon = Instantiate(newWeapon, m_WeaponHolder.position, m_WeaponHolder.rotation, m_WeaponHolder);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
                m_IsGrounded = true;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void shootProjectile()
    {
        if (m_ProjectilePrefab && m_ProjectileSpawnPoint)
        {
            GameObject projectile = Instantiate(m_ProjectilePrefab, m_ProjectileSpawnPoint.position, Quaternion.identity);

            // Determine the direction based on facing direction
            Vector2 shootDirection = transform.localScale.x < 0 ? Vector2.right : Vector2.left;

            // Initialize the projectile
            projectile.GetComponent<Projectile>().Initialize(shootDirection);
        }
        else
        {
            Debug.LogWarning("Weapon prefab or spawn point not assigned.");
        }
    }

}