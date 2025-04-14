using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class MainCharacter : Character
{
    private CharacterMovement characterMovement;
    private CharacterCombat characterCombat;
    private InputSystem_Actions inputActions;
    private Suit equippedSuit;

    [Header("Damage & Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1.0f;

    public bool IsInvincible { get; set; } = false;

    [SerializeField] private BeaconSO beacon;
    
    
    [Header("Visuals")] 
    [SerializeField] private GameObject eye;
    [Header("Sprite Library Settings")]
    [SerializeField] private SpriteLibrary spriteLibrary;
    [SerializeField] private SpriteLibraryAsset normalSpriteLibraryAsset;
    [SerializeField] private SpriteLibraryAsset suitSpriteLibraryAsset;

    
    [Header("Flashing Settings")]
    [SerializeField] private float flashDuration = 1.0f;
    [SerializeField] private float flashInterval = 0.1f;
    private SpriteRenderer sr;
  

    protected override void Awake()
    {
        CurrentFacingDirection = 1;
        base.Awake();
        inputActions = new InputSystem_Actions();
        characterMovement = GetComponent<CharacterMovement>();
        characterCombat = GetComponent<CharacterCombat>();
        sr = GetComponent<SpriteRenderer>();

        
        if (!characterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!characterCombat)
            Debug.LogError("CharacterCombat component is missing.");
        
        if(spriteLibrary != null && normalSpriteLibraryAsset != null)
            spriteLibrary.spriteLibraryAsset = normalSpriteLibraryAsset;
        
    }

    private void Start()
    {
        beacon.uiChannel.ChangeHealth(currentHits);
    }

    
    private void FixedUpdate()
    {
        characterMovement.Move();
        if(transform.localScale.x < 0.1f) CurrentFacingDirection = -1;
    }
    
    private void OnEnable()
    {
        inputActions.Enable();

        // Register Input Action Callbacks
        inputActions.Player.Move.performed += characterMovement.OnMovePerformed;
        inputActions.Player.Move.canceled += characterMovement.OnMoveCanceled;
        inputActions.Player.Jump.performed += _ => characterMovement.Jump();
        inputActions.Player.Jump.canceled += _ => characterMovement.OnJumpReleased();
        inputActions.Player.BasicAttack.performed += _ => PerformBasicAttack();
        inputActions.Player.SpecialAttack.performed += _ => PerformSpecialAttack();
        inputActions.Player.SpecialMove.performed += _ => PerformSpecialMovement();
        inputActions.Player.Consume.performed += _ => UnEquipSuit();
        
    }
   
    
    private void OnDisable()
    {
        inputActions.Disable();
    }
    
    private void PerformBasicAttack()
    {
        Debug.Log($"{gameObject.name} performs a basic attack.dirction {CurrentFacingDirection}");
        characterCombat.BasicAttack(CurrentFacingDirection);
    }
    
    private void PerformSpecialAttack()
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
    
    private void PerformSpecialMovement()
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

    public void UnlockWallGrabAbility()
    {
        characterMovement.canWallGrab = true;
        // visual/audio feedback
    }

    
    public void EquipSuit(Suit newSuit)
    {
        if (equippedSuit != null)
        {
            Heal();
            return;
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

    
    public override void TakeDamage(int damage, float direction)
    {
        if (IsInvincible)
            return;
        Debug.Log($"got hit and has recoil {direction}");
        characterMovement.AddRecoil(direction);
        base.TakeDamage(damage);
        beacon.uiChannel.ChangeHealth(currentHits);
        StartCoroutine(FlashSprite());
        StartCoroutine(InvincibilityCoroutine());
        
    }
    
    private IEnumerator InvincibilityCoroutine()
    {
        IsInvincible = true;
        // Optionally add visual feedback such as blinking the sprite.
        yield return new WaitForSeconds(invincibilityDuration);
        IsInvincible = false;
    }
    
    private void UnEquipSuit()
    {
        eye.SetActive(true);
        if (equippedSuit != null)
        {
            Debug.Log($"Unequipped suit: {equippedSuit.suitName}");
            equippedSuit = null;
            
            if(spriteLibrary != null && normalSpriteLibraryAsset != null)
                spriteLibrary.spriteLibraryAsset = normalSpriteLibraryAsset;
            Heal();
        }
        
    }
    private void Heal()
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
    protected IEnumerator FlashSprite()
    {
        
        if (sr == null)
            yield break;

        float timer = 0f;
        while (timer < flashDuration)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
            yield return new WaitForSeconds(flashInterval);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval * 2;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }
}