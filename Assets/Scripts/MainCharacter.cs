using Scriptable.Scripts;
using UnityEngine;

public class MainCharacter : Character
{
    private CharacterMovement characterMovement;
    private CharacterCombat characterCombat;
    private InputSystem_Actions inputActions;
    private HealthChannelSo healthChannel;
    private GameStateChannel gameStateChannel;
    protected override void Awake()
    {
        base.Awake();
        healthChannel = FindObjectOfType<Beacon>().healthChannel;
        if (healthChannel != null)
            Debug.Log("Found health Channel in main character");
        gameStateChannel = FindObjectOfType<Beacon>().gameStateChannel;
        if (healthChannel != null)
            Debug.Log("Found state Channel in main character");
        characterMovement = GetComponent<CharacterMovement>();
        characterCombat = GetComponent<CharacterCombat>();
        if (!characterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!characterCombat)
            Debug.LogError("CharacterCombat component is missing.");
        inputActions = new InputSystem_Actions(); 
        if(inputActions != null)
            Debug.Log("fond input system");

    }

    void Start()
    {
        healthChannel.ChangeHealth(CurrentHits);
    }
    
    private void Update()
    {

        Vector2 movementInput = inputActions.Player.Move.ReadValue<Vector2>();
        characterMovement.SetHorizontalInput(movementInput);

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
        inputActions.Player.Attack.performed += _ => characterCombat.Shoot();
        inputActions.Player.Menu.performed += _ => gameStateChannel.MenuClicked();
    }
    
    private void OnDisable()
    {
        inputActions.Disable();
    }
    
    public void EquipWeapon(string weaponName)
    {
        if (characterCombat.currentSuit != null)
        {
            Destroy(characterCombat.currentSuit); // Remove the old weapon
        }

        characterCombat.currentSuit = characterCombat.EquipWeapon(weaponName, characterCombat.suitPosition);

        if (characterCombat.currentSuit == null)
        {
            Debug.LogError($"Failed to equip weapon: {weaponName}");
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

    public override void Heal()
    {
        CurrentHits = Mathf.Min(CurrentHits + 1, MaxHits);
        healthChannel.ChangeHealth(CurrentHits);
    }

    
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
    
}