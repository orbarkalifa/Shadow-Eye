using UnityEngine;

namespace Suits.Abilities
{
    [CreateAssetMenu(fileName = "RockForm", menuName = "Ability/Rock Form")]
    public class RockAbility : SuitAbility
    {
        [field:SerializeField] public Sprite RockSprite { get; private set; }
        [field:SerializeField] public Vector2 SmashAreaSize { get; private set; } = new(3f, 1f);
        [field:SerializeField] public Vector2 SmashAreaOffset { get; private set; } = new(0f, -1f);
        [field:SerializeField] public LayerMask EnemyLayerMask { get; private set; }
        [field:SerializeField] public int SmashDamage { get; private set; } = 5;
        [field:SerializeField] public float StunDuration { get; private set; } = 2f;
        [field:SerializeField] public float VelocityDropRate { get; private set; } = 20f; 

        public override void ExecuteAbility(GameObject character)
        {
            var effect = character.GetComponent<RockFormEffect>();
            if (effect == null)
            {
                effect = character.AddComponent<RockFormEffect>();
                effect.Initialize(this);
                effect.Activate();
            }
            else
            {
                effect.DeactivateAndDestroy();
            }
        }
    }
}
