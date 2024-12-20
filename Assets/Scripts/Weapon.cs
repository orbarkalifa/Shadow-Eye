
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    public string m_WeaponName; // Optional: Unique identifier for the weapon
    public GameObject m_WeaponModel; // Optional: Visual representation of the weapon

    private void OnTriggerEnter2D(Collider2D i_Other)
    {
        if (i_Other.CompareTag("Player"))
        {
            Debug.Log($"Player collided with weapon: {m_WeaponName}");
            MainCharacter player = i_Other.GetComponent<MainCharacter>();
            if (player != null)
            {
                Debug.Log($"Player is equipping weapon: {m_WeaponName}");
                player.EquipWeapon(m_WeaponName);
                Destroy(gameObject); // Destroy the pickup after it's collected
            }
        }
    }
}
