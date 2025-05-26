using Player;
using UnityEngine;

namespace Suits.Duri
{
    [CreateAssetMenu(fileName = "RockForm", menuName = "Ability/Rock Form")]
    public class RockAbility : SuitAbility
    {
        [field:SerializeField] public Vector2 SmashAreaSize { get; private set; } = new(3f, 1f);
        [field:SerializeField] public Vector2 SmashAreaOffset { get; private set; } = new(0f, -1f);
        [field:SerializeField] public LayerMask EnemyLayerMask { get; private set; }
        [field:SerializeField] public int SmashDamage { get; private set; } = 5;
        [field:SerializeField] public float StunDuration { get; private set; } = 2f;
        [field:SerializeField] public float VelocityDropRate { get; private set; } = 20f; 
        [field: SerializeField] public AnimationClip TransformInClip { get; private set; }  // New: For transforming IN
        [field: SerializeField] public AnimationClip TransformOutClip { get; private set; } // New: 

        public override void Execute(PlayerController character) // Parameter changed
        {
            // The toggle logic for adding/removing the effect remains.
            // PlayerController will handle the cooldown for "RockAbility" immediately after this.
            var effect = character.GetComponent<RockFormEffect>();
            if (effect == null)
            {
                effect = character.gameObject.AddComponent<RockFormEffect>();
                effect.Initialize(this, character); // Pass this RockAbility SO
                effect.Activate(); // Effect will disable canMove/canAttack
            }
            else
            {
                effect.DeactivateAndDestroy(); // Effect will re-enable canMove/canAttack
            }
        }
        public void TriggerCooldownRequestFromEffect(PlayerController caster)
        {
            RequestCooldownStart(caster);
        }
    }
}
