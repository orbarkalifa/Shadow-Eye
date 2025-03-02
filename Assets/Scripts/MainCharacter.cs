using Scriptable.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCharacter : Character
{
    private CharacterMovement characterMovement;
    private CharacterCombat characterCombat;
    private InputSystem_Actions inputActions;
    private HealthChannelSo healthChannel;
    private GameStateSO GameStateChannel;
    private Suit EquippedSuit;
    
    [FormerlySerializedAs("m_SuitVisualSlot")]
    [Header("Visuals")]
    [SerializeField] Transform SuitVisualSlot; // Slot for the suit visual
    private GameObject CurrentSuitVisual; // Holds the current suit visual instance
    
    private Vector2 FacingDirection = Vector2.right; // Default facing direction

    private InputSystem_Actions InputActions;

    protected override void Awake()
    {
        base.Awake();
        healthChannel = FindObjectOfType<Beacon>().healthChannel;
        if (healthChannel != null)
            Debug.Log("Found health Channel in main character");
        GameStateChannel = FindObjectOfType<Beacon>().GameStateSo;
        if (healthChannel != null)
            Debug.Log("Found state Channel in main character");
        characterMovement = GetComponent<CharacterMovement>();
        characterCombat = GetComponent<CharacterCombat>();
        if (!characterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!characterCombat)
            Debug.LogError("CharacterCombat component is missing.");
        InputActions = new InputSystem_Actions();
        if(inputActions != null)
            Debug.Log("fond input system");
    }

    void Start()
    {
        healthChannel.ChangeHealth(CurrentHits);
    }

    private void Update()
    {
        Vector2 movementInput = InputActions.Player.Move.ReadValue<Vector2>();
        characterMovement.SetHorizontalInput(movementInput);
        FacingDirection = movementInput.x > 0 ? Vector2.left : Vector2.right;
    }
    private void FixedUpdate()
    {
        characterMovement.Move();
    }
    
    private void OnEnable()
    {
        InputActions.Enable();

        // Register Input Action Callbacks
        InputActions.Player.Jump.performed += _ => characterMovement.Jump();
        InputActions.Player.Jump.canceled += _ => characterMovement.OnJumpReleased();
        InputActions.Player.BasicAttack.performed += _ => performBasicAttack();
        InputActions.Player.SpecialAttack.performed += _ => performSpecialAttack();
        InputActions.Player.SpecialMove.performed += _ => performSpecialMovement();
        InputActions.Player.Menu.performed += _ => GameStateChannel.MenuClicked();
    }
    
    private void OnDisable()
    {
        InputActions.Disable();
    }
    
    private void performBasicAttack()
    {
        Debug.Log($"{gameObject.name} performs a basic attack.");
        characterCombat.BasicAttack(FacingDirection);
    }
    
    private void performSpecialAttack()
    {
        if (EquippedSuit?.m_SpecialAttack != null)
        {
            EquippedSuit.m_SpecialAttack.ExecuteAbility(gameObject);
        }
        else
        {
            Debug.LogWarning("No suit equipped or no special attack available.");
        }
    }
    
    private void performSpecialMovement()
    {
        if (EquippedSuit?.m_SpecialMovement != null)
        {
            EquippedSuit.m_SpecialMovement.ExecuteAbility(gameObject);
        }
        else
        {
            Debug.LogWarning("No suit equipped or no special movement available.");
        }

    }
    
    public void EquipSuit(Suit newSuit)
    {
        if (EquippedSuit != null)
        {
            Debug.Log($"Unequipping suit: {EquippedSuit.m_SuitName}");
            destroyCurrentSuitVisual();
        }
        
        EquippedSuit = newSuit;

        if (EquippedSuit != null)
        {
            Debug.Log($"Equipped suit: {EquippedSuit.m_SuitName}");
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
        if (CurrentSuitVisual != null)
        {
            Destroy(CurrentSuitVisual); // Remove the old suit visual
            CurrentSuitVisual = null;
        }
    }

    private void createSuitVisual(Suit suit)
    {
        if (suit.m_SuitPrefab != null) // Assume the Suit ScriptableObject has a `m_SuitPrefab`
        {
            CurrentSuitVisual = Instantiate(suit.m_SuitPrefab, SuitVisualSlot);
            CurrentSuitVisual.transform.localPosition = Vector3.zero; // Reset position to match the slot
            CurrentSuitVisual.transform.localRotation = Quaternion.identity;
            Debug.Log($"Created visual for {suit.m_SuitName}.");
        }
        else
        {
            Debug.LogWarning($"Suit {suit.m_SuitName} has no visual prefab assigned.");
        }
    }
    
    public void UnequipSuit()
    {
        if (EquippedSuit != null)
        {
            Debug.Log($"Unequipped suit: {EquippedSuit.m_SuitName}");
            EquippedSuit = null;
        }
    }
    public void Heal()
    {
        CurrentHits = Mathf.Min(CurrentHits + 1, MaxHits);
        healthChannel.ChangeHealth(CurrentHits);
    }
    
    private void OnDestroy()
    {
        InputActions?.Dispose();
    }
    
}