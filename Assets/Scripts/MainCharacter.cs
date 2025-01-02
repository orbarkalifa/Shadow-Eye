using UnityEngine;

public class MainCharacter : Character
{
    private CharacterMovement characterMovement;
    private CharacterCombat characterCombat;

    private InputSystem_Actions inputActions;

    protected override void Awake()
    {
        base.Awake();
        characterMovement = GetComponent<CharacterMovement>();
        characterCombat = GetComponent<CharacterCombat>();
        if (!characterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!characterCombat)
            Debug.LogError("CharacterCombat component is missing.");
        inputActions = new InputSystem_Actions();

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

    
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
    
}