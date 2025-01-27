using UnityEngine;

public class MainCharacter : Character
{
    private CharacterMovement characterMovement;
    private CharacterCombat characterCombat;
    private InputSystem_Actions inputActions;
    private HealthChannelSo healthChannel;
    private GameStateChannel gameStateChannel;
    private CharacterMovement m_CharacterMovement;
    private CharacterCombat m_CharacterCombat;
    private Suit m_EquippedSuit;
    
    [Header("Visuals")]
    [SerializeField] Transform m_SuitVisualSlot; // Slot for the suit visual
    private GameObject m_CurrentSuitVisual; // Holds the current suit visual instance
    
    private Vector2 m_FacingDirection = Vector2.right; // Default facing direction

    private InputSystem_Actions m_InputActions;

    protected override void Awake()
    {
        base.Awake();
        healthChannel = FindObjectOfType<Beacon>().healthChannel;
        if (healthChannel != null)
            Debug.Log("Found health Channel in main character");
        gameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;
        if (healthChannel != null)
            Debug.Log("Found state Channel in main character");
        m_CharacterMovement = GetComponent<CharacterMovement>();
        m_CharacterCombat = GetComponent<CharacterCombat>();
        if (!m_CharacterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!m_CharacterCombat)
            Debug.LogError("CharacterCombat component is missing.");
        m_InputActions = new InputSystem_Actions();
        if(inputActions != null)
            Debug.Log("fond input system");

    }

    void Start()
    {
        healthChannel.ChangeHealth(CurrentHits);
    }

    private void Update()
    {

        Vector2 movementInput = m_InputActions.Player.Move.ReadValue<Vector2>();
        m_CharacterMovement.SetHorizontalInput(movementInput);
        
        m_FacingDirection = movementInput.x > 0 ? Vector2.left : Vector2.right;

    }
    private void FixedUpdate()
    {
        m_CharacterMovement.Move();
    }
    
    private void OnEnable()
    {
        m_InputActions.Enable();

        // Register Input Action Callbacks
        m_InputActions.Player.Jump.performed += _ => m_CharacterMovement.Jump();
        m_InputActions.Player.Jump.canceled += _ => m_CharacterMovement.OnJumpReleased();
        m_InputActions.Player.BasicAttack.performed += _ => performBasicAttack();
        m_InputActions.Player.SpecialAttack.performed += _ => performSpecialAttack();
        m_InputActions.Player.SpecialMove.performed += _ => performSpecialMovement();
        m_InputActions.Player.Menu.performed += _ => gameStateChannel.MenuClicked();
    }
    
    private void OnDisable()
    {
        m_InputActions.Disable();
    }
    
    private void performBasicAttack()
    {
        Debug.Log($"{gameObject.name} performs a basic attack.");
        m_CharacterCombat.BasicAttack(m_FacingDirection);
    }
    
    private void performSpecialAttack()
    {
        if (m_EquippedSuit?.m_SpecialAttack != null)
        {
            m_EquippedSuit.m_SpecialAttack.ExecuteAbility(gameObject);
        }
        else
        {
            Debug.LogWarning("No suit equipped or no special attack available.");
        }
    }
    
    private void performSpecialMovement()
    {
        if (m_EquippedSuit?.m_SpecialMovement != null)
        {
            m_EquippedSuit.m_SpecialMovement.ExecuteAbility(gameObject);
        }
        else
        {
            Debug.LogWarning("No suit equipped or no special movement available.");
        }

    }
    
    public void EquipSuit(Suit newSuit)
    {
        if (m_EquippedSuit != null)
        {
            Debug.Log($"Unequipping suit: {m_EquippedSuit.m_SuitName}");
            destroyCurrentSuitVisual();
        }

        m_EquippedSuit = newSuit;

        if (m_EquippedSuit != null)
        {
            Debug.Log($"Equipped suit: {m_EquippedSuit.m_SuitName}");
            createSuitVisual(newSuit);
        }
    }
    
    public override void TakeDamage(int damage)
    {
        CurrentHits -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {CurrentHits}");
        healthChannel.ChangeHealth(CurrentHits);
        if (CurrentHits <= 0)
        {
            OnDeath();
        }
    }

    private void destroyCurrentSuitVisual()
    {
        if (m_CurrentSuitVisual != null)
        {
            Destroy(m_CurrentSuitVisual); // Remove the old suit visual
            m_CurrentSuitVisual = null;
        }
    }

    private void createSuitVisual(Suit suit)
    {
        if (suit.m_SuitPrefab != null) // Assume the Suit ScriptableObject has a `m_SuitPrefab`
        {
            m_CurrentSuitVisual = Instantiate(suit.m_SuitPrefab, m_SuitVisualSlot);
            m_CurrentSuitVisual.transform.localPosition = Vector3.zero; // Reset position to match the slot
            m_CurrentSuitVisual.transform.localRotation = Quaternion.identity;
            Debug.Log($"Created visual for {suit.m_SuitName}.");
        }
        else
        {
            Debug.LogWarning($"Suit {suit.m_SuitName} has no visual prefab assigned.");
        }
    }
    
    public void UnequipSuit()
    {
        if (m_EquippedSuit != null)
        {
            Debug.Log($"Unequipped suit: {m_EquippedSuit.m_SuitName}");
            m_EquippedSuit = null;
        }
    }
    public override void Heal()
    {
        CurrentHits = Mathf.Min(CurrentHits + 1, MaxHits);
        healthChannel.ChangeHealth(CurrentHits);
    }

    
    private void OnDestroy()
    {
        m_InputActions?.Dispose();
    }
    
}