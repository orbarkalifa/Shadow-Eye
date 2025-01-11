using UnityEngine;

public class MainCharacter : Character
{
    private CharacterMovement m_CharacterMovement;
    private CharacterCombat m_CharacterCombat;
    [SerializeField]private Suit m_EquippedSuit;
    
    [Header("Visuals")]
    [SerializeField] Transform m_SuitVisualSlot; // Slot for the suit visual
    private GameObject m_CurrentSuitVisual; // Holds the current suit visual instance
    
    private Vector2 facingDirection = Vector2.left; // Default facing direction

    private InputSystem_Actions m_InputActions;

    protected override void Awake()
    {
        base.Awake();
        m_CharacterMovement = GetComponent<CharacterMovement>();
        m_CharacterCombat = GetComponent<CharacterCombat>();
        if (!m_CharacterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!m_CharacterCombat)
            Debug.LogError("CharacterCombat component is missing.");
        m_InputActions = new InputSystem_Actions();

    }

    private void Update()
    {

        Vector2 movementInput = m_InputActions.Player.Move.ReadValue<Vector2>();
        m_CharacterMovement.SetHorizontalInput(movementInput);
        
        if (movementInput.x < 0)
            facingDirection = Vector2.left;
        else if (movementInput.x > 0)
            facingDirection = Vector2.right;
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
    }
    
    private void OnDisable()
    {
        m_InputActions.Disable();
    }
    
    private void performBasicAttack()
    {
        Debug.Log($"{gameObject.name} performs a basic attack.");
        m_CharacterCombat.BasicAttack(facingDirection);
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
    
    /*public void EquipWeapon(string weaponName)
    {
        if (m_CharacterCombat.currentSuit != null)
        {
            Destroy(m_CharacterCombat.currentSuit); // Remove the old weapon
        }

        m_CharacterCombat.currentSuit = m_CharacterCombat.EquipWeapon(weaponName, m_CharacterCombat.suitPosition);

        if (m_CharacterCombat.currentSuit == null)
        {
            Debug.LogError($"Failed to equip weapon: {weaponName}");
        }
    }*/

    
    private void OnDestroy()
    {
        m_InputActions?.Dispose();
    }
    
}