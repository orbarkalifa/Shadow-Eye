using GameStateManagement;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class MainCharacter : Character
{
    private CharacterMovement characterMovement;
    private CharacterCombat characterCombat;
    private InputSystem_Actions inputActions;
    [SerializeField] private BeaconSO beacon;
    private Suit equippedSuit;
    
    [Header("Visuals")] 
    public Vector2 facingDirection = Vector2.right; // Default facing direction
    [SerializeField] private GameObject eye;
    [Header("Sprite Library Settings")]
    [SerializeField] private SpriteLibrary spriteLibrary;
    [SerializeField] private SpriteLibraryAsset normalSpriteLibraryAsset;
    [SerializeField] private SpriteLibraryAsset suitSpriteLibraryAsset;

  

    protected override void Awake()
    {
        base.Awake();
        inputActions = new InputSystem_Actions();
        characterMovement = GetComponent<CharacterMovement>();
        characterCombat = GetComponent<CharacterCombat>();
        
        if (!characterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!characterCombat)
            Debug.LogError("CharacterCombat component is missing.");
        
        if(spriteLibrary != null && normalSpriteLibraryAsset != null)
            spriteLibrary.spriteLibraryAsset = normalSpriteLibraryAsset;
        
    }

    void Start()
    {
        beacon.uiChannel.ChangeHealth(currentHits);
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
        inputActions.Player.Consume.performed += _ => unEquipSuit();
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
        }
        
        equippedSuit = newSuit;

        if (equippedSuit != null)
        {
            if(spriteLibrary != null && suitSpriteLibraryAsset != null)
            {
                eye.SetActive(false);
                spriteLibrary.spriteLibraryAsset = suitSpriteLibraryAsset;
            }
        }
    }

    
    public override void TakeDamage(int damage)
    {
        currentHits -= damage;
        beacon.uiChannel.ChangeHealth(currentHits);
        if (currentHits <= 0)
        {
            OnDeath();
        }
    }
    
    private void unEquipSuit()
    {
        eye.SetActive(true);
        if (equippedSuit != null)
        {
            Debug.Log($"Unequipped suit: {equippedSuit.suitName}");
            equippedSuit = null;
            
            // Revert sprite library back to normal
            if(spriteLibrary != null && normalSpriteLibraryAsset != null)
                spriteLibrary.spriteLibraryAsset = normalSpriteLibraryAsset;
            Heal();
        }
        
    }
    public void Heal()
    {
        currentHits = Mathf.Min(currentHits + 1, maxHits);
        beacon.uiChannel.ChangeHealth(currentHits);
    }
    
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    protected override void OnDeath()
    {
        var gameOverState = beacon.gameStateChannel.GetGameStateByName("Game Over");
        if (beacon.gameStateChannel && gameOverState != null)
        {
            Debug.Log($"Game Over: {gameOverState.name}");
            beacon.gameStateChannel.RaiseStateTransitionRequest(gameOverState);
        }
        else
        {
            Debug.LogError("PlayerHealth: Beacon, GameStateChannel, or GameOverState is not assigned!");
        }
        base.OnDeath();
    }
}