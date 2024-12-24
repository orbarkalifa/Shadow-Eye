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
        /*// Handle inputs
        characterMovement.Jump();

        if (Input.GetButtonDown("Fire1"))
            characterCombat.shoot();*/
        
        // Get movement input
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
        inputActions.Player.Attack.performed += _ => characterCombat.Shoot();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    
    public void EquipWeapon(string weaponName)
    {
        if (currentSuit != null)
        {
            Destroy(currentSuit); // Remove the old weapon
        }

        currentSuit = characterCombat.EquipWeapon(weaponName, suitPosition);

        if (currentSuit == null)
        {
            Debug.LogError($"Failed to equip weapon: {weaponName}");
        }
    }
    
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
    
}