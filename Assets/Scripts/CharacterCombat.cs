using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject g_ProjectilePrefab;
    [SerializeField] private Transform m_FirePoint;
    [SerializeField] List<GameObject> m_WeaponPrefabs;
    public GameObject currentSuit; // Reference to the player's active weapon
    public Transform suitPosition; // Transform where weapons are attached to the player
    
    // Start is called before the first frame update
    public void Shoot()
    {
        
        if (g_ProjectilePrefab && m_FirePoint)
        {
            // Instantiate projectile and shoot in the direction the character is facing
            GameObject projectile = Instantiate(g_ProjectilePrefab, m_FirePoint.position, Quaternion.identity);
            Vector2 shootDirection = transform.localScale.x == -1 ? Vector2.right : Vector2.left;
            projectile.GetComponent<Projectile>().Initialize(shootDirection);
        }
        else
        {
            Debug.LogWarning("Projectile prefab or fire point not assigned.");
        }
    }
    
    public GameObject EquipWeapon(string weaponName, Transform weaponHolder)
    {
        foreach (GameObject weapon in m_WeaponPrefabs)
        {
            if (weapon.name == weaponName)
            {
                Vector3 weaponPosition = new Vector3(weaponHolder.position.x, weaponHolder.position.y, -1);
                GameObject newWeapon = Instantiate(weapon, weaponPosition, weaponHolder.rotation, weaponHolder);
                return newWeapon;
            }
        }
        Debug.LogWarning($"Weapon '{weaponName}' not found.");
        return null;
    }
    
    public GameObject GetWeaponByName(string i_WeaponName)
    {
        foreach (GameObject weapon in m_WeaponPrefabs)
        {
            if (weapon.name == i_WeaponName)
            {
                return weapon;
            }
        }
        return null;
    }
}
