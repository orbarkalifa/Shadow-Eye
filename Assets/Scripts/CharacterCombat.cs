using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject g_ProjectilePrefab;
    [SerializeField] private Transform m_FirePoint;
    // Start is called before the first frame update
    public void shoot()
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
}
