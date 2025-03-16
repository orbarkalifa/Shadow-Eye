using Scriptable.Scripts;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class MainCharacter : Character
{
    private CharacterMovement characterMovement;
    private CharacterCombat characterCombat;
    private InputSystem_Actions inputActions;
    private HealthChannelSo healthChannel;
    private GameStateChannel gameStateChannel;
    private Suit equippedSuit;
    
    [Header("Visuals")]
    [SerializeField] Transform suitVisualSlot; // Slot for the suit visual
    private GameObject currentSuitVisual; // Holds the current suit visual instance
    private Vector2 facingDirection = Vector2.right; // Default facing direction
    
    [Header("Sprite Library Settings")]
    [SerializeField] private SpriteLibrary spriteLibrary;
    [SerializeField] private SpriteLibraryAsset normalSpriteLibraryAsset;
    [SerializeField] private SpriteLibraryAsset suitSpriteLibraryAsset;
  

    protected override void Awake()
    {
        base.Awake();
        inputActions = new InputSystem_Actions();
        healthChannel = FindObjectOfType<Beacon>().healthChannel;
        gameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;
        characterMovement = GetComponent<CharacterMovement>();
        characterCombat = GetComponent<CharacterCombat>();
        
        if (!characterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!characterCombat)
            Debug.LogError("CharacterCombat component is missing.");
        
        // Set the default sprite library to normal
        if(spriteLibrary != null && normalSpriteLibraryAsset != null)
            spriteLibrary.spriteLibraryAsset = normalSpriteLibraryAsset;
        
    }

    void Start()
    {
        healthChannel.ChangeHealth(currentHits);
    }

    private void Update()
    {
        Vector2 movementInput = inputActions.Player.Move.ReadValue<Vector2>();
        characterMovement.SetHorizontalInput(movementInput);
        facingDirection = movementInput.x < 0 ? Vector2.left : Vector2.right;
    }
    private void FixedUpdate()
    {
        characterMovement.Move();
    }
    
    private void OnEnable()
    {
        inputActions.Enable();

        // Register Input Action Callbacks
        inputActions.Player.Jump.performed += _ => characterMovement.Jump();
        inputActions.Player.Jump.canceled += _ => characterMovement.OnJumpReleased();
        inputActions.Player.BasicAttack.performed += _ => performBasicAttack();
        inputActions.Player.SpecialAttack.performed += _ => performSpecialAttack();
        inputActions.Player.SpecialMove.performed += _ => performSpecialMovement();
        inputActions.Player.Menu.performed += _ => gameStateChannel.MenuClicked();
    }
    
    private void OnDisable()
    {
        inputActions.Disable();
    }
    
    private void performBasicAttack()
    {
        Debug.Log($"{gameObject.name} performs a basic attack.");
        characterCombat.BasicAttack(facingDirection);
    }
    
    private void performSpecialAttack()
    {
        if (equippedSuit?.specialAttack != null)
        {
            equippedSuit.specialAttack.ExecuteAbility(gameObject);
        }
        else
        {
            Debug.LogWarning("No suit equipped or no special attack available.");
        }
    }
    
    private void performSpecialMovement()
    {
        if (equippedSuit?.specialMovement != null)
        {
            
            equippedSuit.specialMovement.ExecuteAbility(gameObject);
        }
        else
        {
            Debug.LogWarning("No suit equipped or no special movement available.");
        }

    }
    
    public void EquipSuit(Suit newSuit)
    {
        if (equippedSuit != null)
        {
            Debug.Log($"Unequipping suit: {equippedSuit.suitName}");
            destroyCurrentSuitVisual();
        }
        
        equippedSuit = newSuit;

        if (equippedSuit != null)
        {
            /*createSuitVisual(newSuit);*/
            
            // In EquipSuit method after setting the suit sprite library asset
            if(spriteLibrary != null && suitSpriteLibraryAsset != null)
            {
                spriteLibrary.spriteLibraryAsset = suitSpriteLibraryAsset;
            }
        }
    }

    
    public override void TakeDamage(int damage)
    {
        currentHits -= damage;
        healthChannel.ChangeHealth(currentHits);
        if (currentHits <= 0)
        {
            OnDeath();
        }
    }

    private void destroyCurrentSuitVisual()
    {
        if (currentSuitVisual != null)
        {
            Destroy(currentSuitVisual); // Remove the old suit visual
            currentSuitVisual = null;
        }
    }

    /*private void createSuitVisual(Suit suit)
    {
        if (suit.suitPrefab != null)
        {
            currentSuitVisual = Instantiate(suit.suitPrefab, suitVisualSlot);
            currentSuitVisual.transform.localPosition = Vector3.zero;
            currentSuitVisual.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning($"Suit {suit.suitName} has no visual prefab assigned.");
        }
    }*/
    
    public void UnequipSuit()
    {
        if (equippedSuit != null)
        {
            Debug.Log($"Unequipped suit: {equippedSuit.suitName}");
            equippedSuit = null;
            
            // Revert sprite library back to normal
            if(spriteLibrary != null && normalSpriteLibraryAsset != null)
                spriteLibrary.spriteLibraryAsset = normalSpriteLibraryAsset;
            destroyCurrentSuitVisual();
            
        }
        
    }
    public void Heal()
    {
        currentHits = Mathf.Min(currentHits + 1, maxHits);
        healthChannel.ChangeHealth(currentHits);
    }
    
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
    
}