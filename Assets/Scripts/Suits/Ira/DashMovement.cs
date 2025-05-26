using Player;
using UnityEngine;

namespace Suits.Ira
{
    [CreateAssetMenu(fileName = "Dash", menuName = "Ability/Dash")]
    public class DashMovement : SuitAbility
    {
        public override void Execute(PlayerController character)
        {
            CharacterMovement movement = character.GetComponent<CharacterMovement>();
            if (movement != null && movement.canMove)
            {
                movement.Dash();
                RequestCooldownStart(character);
            }
            else if (movement == null)
            {
                Debug.LogError("DashMovement: CharacterMovement component not found on character.");
            }
        }
    }
}