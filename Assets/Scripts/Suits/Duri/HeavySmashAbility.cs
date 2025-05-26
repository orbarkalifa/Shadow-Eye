using UnityEngine;
using EnemyAI;

namespace Suits.Duri
{
    [CreateAssetMenu(fileName = "HeavySmash", menuName = "Ability/Heavy Smash")]
    public class HeavySmashAbility : SuitAbility
    {
        [Header("AOE Settings")]
        [SerializeField] Vector2 attackSize = new (4f,1f);
        [SerializeField] Vector2 attackOffset = new (1f,0f);
        [SerializeField] LayerMask enemyLayer= ~0;
        [SerializeField] int smashDamage = 2;
        [Header("Bounce & Damage")]
        [SerializeField] float bounceForce  = 10f;

        MainCharacter main;
        CharacterCombat combat;

        public override void ExecuteAbility(GameObject character)
        {
            Debug.Log("HeavySmashAbility.ExecuteAbility called.");

            if (main == null) // This will only run the first time
            {
                main = character.GetComponent<MainCharacter>();
                combat = character.GetComponent<CharacterCombat>();
                Debug.Log($"HeavySmash: Initialized main ({main != null}) and combat ({combat != null}).");
            }

            if (main == null || combat == null)
            {
                Debug.LogError("HeavySmash: MainCharacter or CharacterCombat component is missing!");
                return;
            }

            main.animator.CrossFadeInFixedTime("HeavySmash", 0.1f);
            
        }
        public void ApplySmashEffect(GameObject character)
        {
            float dir = main.CurrentFacingDirection;
            Vector2 center = (Vector2)character.transform.position
                             + new Vector2(attackOffset.x * dir, attackOffset.y);


            Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackSize, 0f, enemyLayer);

            foreach (var col in hits)
            {
                if (col.TryGetComponent(out Enemy e))
                {
                    float recoilDir = Mathf.Sign(e.transform.position.x - character.transform.position.x);
                    if (recoilDir == 0) recoilDir = (dir > 0 ? 1 : -1); // Ensure a direction if perfectly aligned
                    e.TakeDamage(smashDamage, recoilDir); // Using CharacterCombat's base attack damage
                    e.rb.AddForce(Vector2.up * bounceForce * e.rb.mass, ForceMode2D.Impulse);
                }
            }        }

    #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            var mc = FindObjectOfType<MainCharacter>();
            if (mc == null) return;
            float dir = mc.CurrentFacingDirection;
            Vector2 center = (Vector2)mc.transform.position
                           + new Vector2(attackOffset.x * dir, attackOffset.y);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, attackSize);
        }
    #endif
    }


}