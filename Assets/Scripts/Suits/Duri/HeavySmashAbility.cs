using UnityEngine;
using EnemyAI;
using Player;

namespace Suits.Duri
{
    [CreateAssetMenu(fileName = "HeavySmash", menuName = "Ability/Heavy Smash")]
    public class HeavySmashAbility : SuitAbility
    {
        [Header("AOE Settings")]
        [SerializeField] private Vector2 attackHitboxSize = new(4f, 2f); 
        [SerializeField] private Vector2 attackHitboxOffset = new(1f, 0f); 
        [SerializeField] private LayerMask enemyLayer = ~0; 
        [SerializeField] private int smashDamage = 2;

        [Header("Effects")]
        [SerializeField] private float bounceForce = 10f;

        public override void Execute(PlayerController caster)
        {
            Animator animator = caster.GetComponent<Animator>();
            if (animator != null)
            {
                animator.CrossFadeInFixedTime("HeavySmash", 0.1f, 0, 0f);
                RequestCooldownStart(caster);
            }
            else
            {
                Debug.LogError($"{abilityName}: Animator not found on caster {caster.name}!");
            }
        }
        public void ApplySmashEffect(PlayerController caster)
        {
            if (caster == null)
            {
                Debug.LogError($"{abilityName}: Caster is null in ApplySmashEffectAndRequestCooldown.");
                return;
            }

            float facingDirection = caster.CurrentFacingDirection;
            Vector2 originPoint = caster.transform.position;

            Vector2 hitboxCenter = originPoint + new Vector2(attackHitboxOffset.x * facingDirection, 0);

            Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter, attackHitboxSize, 0f, enemyLayer);
            
            caster.ImpulseCamera();

            foreach (var collision in hits)
            {
                if (collision.TryGetComponent(out Enemy enemy))
                {
                    float recoilDir = Mathf.Sign(enemy.transform.position.x - caster.transform.position.x);
                    if (recoilDir == 0) recoilDir = facingDirection; 

                    enemy.TakeDamage(smashDamage, recoilDir);
                    if (enemy.rb != null && bounceForce > 0) 
                    {
                        enemy.rb.AddForce(Vector2.up * bounceForce * enemy.rb.mass, ForceMode2D.Impulse);
                    }
                }
                if (collision.TryGetComponent<Destructible>(out var d))
                {
                    d.TakeDamage(smashDamage);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (UnityEditor.Selection.activeObject != this) return; 

            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                DrawAbilityGizmos(player); 
            }
            else
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(attackHitboxOffset, attackHitboxSize); // Draw relative to origin
                UnityEditor.Handles.Label(attackHitboxOffset + Vector2.up, $"{abilityName} (No Player in Scene)");
            }
        }

        public void DrawAbilityGizmos(PlayerController player)
        {
            if (player == null) return;

            float facingDirection = player.transform.localScale.x > 0 ? 1f : -1f;
            Vector2 originPoint = player.transform.position;
            Vector2 hitboxCenter = originPoint + new Vector2(attackHitboxOffset.x * facingDirection, attackHitboxOffset.y);

            Gizmos.color = new Color(0.0f, 0.8f, 0.8f, 0.5f);
            Gizmos.DrawWireCube(hitboxCenter, attackHitboxSize);
        }
#endif
    }


}