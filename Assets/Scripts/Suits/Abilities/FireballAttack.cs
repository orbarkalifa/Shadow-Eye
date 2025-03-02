namespace Suits.Abilities
{
    using UnityEngine;
    using UnityEngine.Serialization;

    public class FireballAttack : SuitAbility
    {
        [SerializeField] private float Lifetime = 2f;
        public GameObject FireballPrefab;

        public override void ExecuteAbility(GameObject character)
        {
            Transform firePoint = character.transform.Find("Weapon Spawn Point"); 
            if (FireballPrefab != null && firePoint != null)
            {
                float facingDirection = character.transform.localScale.x < 0 ? -1 : 1;

                GameObject fireball = Instantiate(FireballPrefab, firePoint.position, Quaternion.identity);


                fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(facingDirection * 300f, 0);
                
                Destroy(fireball, Lifetime);

            }
            else
            {
                Debug.LogWarning("Projectile prefab or fire point not assigned.");
            }
        }
    }    
}

