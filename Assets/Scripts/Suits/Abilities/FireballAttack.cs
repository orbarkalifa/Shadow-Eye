namespace Suits.Abilities
{
    using UnityEngine;
    using UnityEngine.Serialization;

    [CreateAssetMenu(fileName = "FireballAttack", menuName = "Suits/Abilities/Fireball Attack")]
    public class FireballAttack : SuitAbility
    {
        public GameObject m_FireballPrefab;

        public override void ExecuteAbility(GameObject character)
        {
            Transform firePoint = character.transform.Find("Weapon Spawn Point"); 
            if (m_FireballPrefab != null && firePoint != null)
            {
                float facingDirection = character.transform.localScale.x < 0 ? 1 : -1;

                GameObject fireball = Instantiate(m_FireballPrefab, firePoint.position, Quaternion.identity);

                fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(facingDirection * 50f, 0); 

            }
            else
            {
                Debug.LogWarning("Projectile prefab or fire point not assigned.");
            }
        }
    }    
}

