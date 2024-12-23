using UnityEngine;
using UnityEngine.Serialization;

public class MainCharacter : Character
{
    private CharacterMovement characterMovement;
    private CharacterCombat combat;


    protected override void Awake()
    {
        base.Awake();
        characterMovement = GetComponent<CharacterMovement>();
        combat = GetComponent<CharacterCombat>();
        if (!characterMovement)
            Debug.LogError("CharacterMovement component is missing.");
        if (!combat)
            Debug.LogError("CharacterCombat component is missing.");
    }

    private void Update()
    {
        // Handle inputs
        characterMovement.GetHorizontalInput();
        characterMovement.Jump();

        if (Input.GetButtonDown("Fire1"))
            combat.shoot();
    }
    private void FixedUpdate()
    {
        characterMovement.Move();
        
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
            Debug.Log("Weapon is not null");
            Vector3 position = new Vector3(m_WeaponHolder.position.x, m_WeaponHolder.position.y, -1);
            m_CurrentWeapon = Instantiate(newWeapon, position, m_WeaponHolder.rotation, m_WeaponHolder);
        }
        if (m_CurrentWeapon == null)
            Debug.LogError("m_CurrentWeapon weapon is null");
    }
    





}