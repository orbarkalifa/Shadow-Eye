using Player;
using UnityEngine;

namespace Suits
{
    public abstract class SuitAbility : ScriptableObject
    {
        [Header("Ability Info")]
        public string abilityName = "Unnamed Ability";

        [Header("Gameplay")]
        public float cooldownTime = 1f;

        /// <summary>
        /// Executes the core logic of this ability.
        /// PlayerController will handle starting the cooldown after this returns.
        /// </summary>
        /// <param name="character">The PlayerController casting this ability.</param>
        public abstract void Execute(PlayerController character);
        
        /// <summary>
        /// Abilities call this method to tell the PlayerController to start tracking their cooldown.
        /// </summary>
        protected void RequestCooldownStart(PlayerController character)
        {
            if (character == null) return;
            character.StartTrackingCooldown(this); // Call public method on PlayerController
        }

    }
}