using UnityEngine;
using EnemyAI;

namespace Suits.Duri
{
    [CreateAssetMenu(fileName = "HeavySmash", menuName = "Ability/Heavy Smash")]
    public class HeavySmashAbility : SuitAbility
    {
        [Header("AOE Settings")]
        [SerializeField] Vector2  attackSize   = new Vector2(4f,1f);
        [SerializeField] Vector2  attackOffset = new Vector2(1f,0f);
        [SerializeField] LayerMask enemyLayer  = ~0;

        [Header("Bounce & Damage")]
        [SerializeField] float    bounceForce  = 10f;

        // — cached on first ExecuteAbility call —
        MainCharacter   _main;
        CharacterCombat _combat;
        Rigidbody2D     _rb;

        public override void ExecuteAbility(GameObject character)
        {
            // cache once
            if (_main == null)
            {
                _main   = character.GetComponent<MainCharacter>();
                _combat = character.GetComponent<CharacterCombat>();
                _rb     = character.GetComponent<Rigidbody2D>();
            }

            // build box centre in front of player
            float dir = _main.CurrentFacingDirection;
            Vector2 center = (Vector2)character.transform.position
                           + new Vector2(attackOffset.x * dir, attackOffset.y);

            // overlap & hit
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackSize, 0f, enemyLayer);
            foreach (var col in hits)
            {
                if (col.TryGetComponent<Enemy>(out var e))
                {
                    // deal damage using your existing basic‐attack damage
                    float recoilDir = Mathf.Sign(e.transform.position.x - character.transform.position.x);
                    e.TakeDamage(_combat.attackDamage, recoilDir);

                    // bounce them up
                    e.rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                }
            }
        }

    #if UNITY_EDITOR
        // draws the box in Scene view for tweaking
        void OnDrawGizmosSelected()
        {
            // try to find your player in the scene (only for editor preview)
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