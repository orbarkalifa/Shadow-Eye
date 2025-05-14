using UnityEngine;

namespace Suits.Abilities
{
    public class RockAbility : SuitAbility
    {
        public override void ExecuteAbility(GameObject character)
        {
            BecomeRock();
            if (wasMidAir)
                SmashGround();
        }

        private void BecomeRock()
        {
            ChangeSprite();
            BecomeInvincible();
        }

        private void SmashGround()
        {
            DamagePlatform();
            StunEnemies();
        }
        
    }
}