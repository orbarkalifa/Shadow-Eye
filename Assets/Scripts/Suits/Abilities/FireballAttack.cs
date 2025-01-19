namespace Suits.Abilities
{
    using UnityEngine;
    using UnityEngine.Serialization;

    public class FireballAttack : SuitAbility
    {
        [SerializeField] private float m_Lifetime = 2f;
        public GameObject m_FireballPrefab;

        public override void ExecuteAbility(GameObject character)
        {
            Transform firePoint = character.transform.Find("Weapon Spawn Point"); 
            if (m_FireballPrefab != null && firePoint != null)
            {
                float facingDirection = character.transform.localScale.x < 0 ? 1 : -1;

                GameObject fireball = Instantiate(m_FireballPrefab, firePoint.position, Quaternion.identity);


                fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(facingDirection * 50f, 0);
                
                Destroy(fireball, m_Lifetime);

            }
            else
            {
                Debug.LogWarning("Projectile prefab or fire point not assigned.");
            }
        }
    }    
}

