namespace Suits.Abilities
{
    using UnityEngine;
    using UnityEngine.Serialization;

    [CreateAssetMenu(fileName = "FireballAttack", menuName = "Suits/Abilities/Fireball Attack")]
    public class FireballAttack : SuitAbility
    {
        public GameObject m_FireballPrefab;
        public Transform m_FirePoint;

        public override void ExecuteAbility(GameObject character)
        {
            if (m_FireballPrefab != null && m_FirePoint != null)
            {
                float facingDirection = character.transform.localScale.x < 0 ? 1 : -1;

                GameObject fireball = Instantiate(m_FireballPrefab, m_FirePoint.position, Quaternion.identity);

                fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(facingDirection * 50f, 0); // Adjust speed as needed

            }
            else
            {
                Debug.LogWarning("Projectile prefab or fire point not assigned.");
            }
        }
    }    
}

