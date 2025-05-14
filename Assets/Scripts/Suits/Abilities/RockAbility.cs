using UnityEngine;

namespace Suits.Abilities
{
    [CreateAssetMenu(fileName = "RockForm", menuName = "Ability/Rock Form")]
    public class RockAbility : SuitAbility
    {
        [SerializeField]private Sprite rockSprite;
        MainCharacter player;
        public override void ExecuteAbility(GameObject character)
        {
            player = character.GetComponent<MainCharacter>();
            BecomeRock();
            /*if (!player.IsGrounded())
                SmashGround();*/
    
            
        }

        private void BecomeRock()
        {
            ChangeSprite();
            BecomeInvincible();
            player.ToggleControls();
        }

        /*private void SmashGround()
        {
            DamagePlatform();
            StunEnemies();
        }*/
        private void ChangeSprite()
        {
            player.ChangeSprite(rockSprite);
        }

        private void BecomeInvincible()
        {
            player.ChangeInvincibleState();
        }

    }
}