using Player;

namespace Suits.Ira
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Projectile", menuName = "Ability/Projectile")]
    public class FireballAttack : SuitAbility
    {
        [SerializeField] private float lifetime = 2f;
        public GameObject fireballPrefab;

        public override void Execute(PlayerController character)
        {
            Transform firePoint = character.transform.Find("Weapon Spawn Point"); 
            if (fireballPrefab != null && firePoint != null)
            {
                float facingDirection = character.transform.localScale.x < 0 ? -1 : 1;
                GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
                if(facingDirection == -1)
                    fireball.transform.localScale = new Vector3(-fireball.transform.localScale.x, fireball.transform.localScale.y,0);

                fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(facingDirection * 40f, 0);
                
                Destroy(fireball, lifetime);
                
                RequestCooldownStart(character);

            }
            else
            {
                Debug.LogWarning("Projectile prefab or fire point not assigned.");
            }
        }
    }    
}

