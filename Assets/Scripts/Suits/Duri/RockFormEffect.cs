using System.Collections;
using Cinemachine;
using EnemyAI;
using Player;
using UnityEngine;

namespace Suits.Duri
{
    public class RockFormEffect : MonoBehaviour
    {
        private RockAbility rockAbilitySO;
        PlayerController player;
        CharacterMovement movement;
        CharacterCombat combat;
        Rigidbody2D rb;
        Animator animator;

        bool origInvincible;
        Coroutine activeRockFormCoroutine;
        Coroutine velocityCoroutine;
        bool waitForLanding;
        float transformInClipLength;
        float transformOutClipLength; 
        

        public void Initialize(RockAbility config, PlayerController caster)
        {
            player = caster;
            movement = caster.characterMovement;
            combat = caster.characterCombat;
            rockAbilitySO = config; 
            rb = player.rb;
            animator = player.animator;

            if (rockAbilitySO.TransformInClip == null)
            {
                Debug.LogError("RockFormEffect: TransformInClip is not assigned in RockAbility config!", rockAbilitySO);
                transformInClipLength = 0f;
                enabled = false; 
                return;
            }
            transformInClipLength = rockAbilitySO.TransformInClip.length;

            if (rockAbilitySO.TransformOutClip == null)
            {
                Debug.LogWarning("RockFormEffect: TransformOutClip is not assigned. Reverting animation will be skipped.", rockAbilitySO);
                transformOutClipLength = 0f;
            }
            else
            {
                transformOutClipLength = rockAbilitySO.TransformOutClip.length;
            }
        }

        public void Activate()
        {
            if (!enabled) return; 

            origInvincible = player.IsInvincible;
            movement.canMove = false;
            combat.canAttack = false;
            player.ChangeInvincibleState(true);

            if (velocityCoroutine != null) StopCoroutine(velocityCoroutine);
            velocityCoroutine = StartCoroutine(DropVelocity());

            if (activeRockFormCoroutine != null) StopCoroutine(activeRockFormCoroutine); // Stop if somehow called again
            activeRockFormCoroutine = StartCoroutine(ManageRockFormAnimation());
        }

        IEnumerator ManageRockFormAnimation()
        {
            // Phase 1: Play Transform-In Animation
            animator.speed = 1f;
            animator.CrossFadeInFixedTime(rockAbilitySO.TransformInClip.name, 0f);
            yield return new WaitForSeconds(transformInClipLength+0.1f);

            // Phase 2: Hold Last Frame (by setting speed to 0 after 'In' animation)
            animator.speed = 0f; // Freeze animation at the last frame of TransformInClip

            waitForLanding = !player.IsGrounded();
            if (player.IsGrounded() && !waitForLanding)
            {
                Smash();
            }
        }

        void Update()
        {
            if (waitForLanding && player.IsGrounded())
            {
                Smash();
                waitForLanding = false;
            }
        }

        void FixedUpdate()
        {
            if (waitForLanding)
            {
                rb.velocity += Vector2.down * (rockAbilitySO.VelocityDropRate * 0.1f * Time.fixedDeltaTime);
            }
        }

        IEnumerator DropVelocity()
        {
            while (Mathf.Abs(rb.velocity.x) > 0.05f)
            {
                float newX = Mathf.MoveTowards(rb.velocity.x, 0f, rockAbilitySO.VelocityDropRate * Time.deltaTime);
                rb.velocity = new Vector2(newX, rb.velocity.y);
                yield return null;
            }
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        void Smash()
        {
            Vector2 center = (Vector2)transform.position + rockAbilitySO.SmashAreaOffset;
            var attackable = Physics2D.OverlapBoxAll(center, rockAbilitySO.SmashAreaSize, 0f, rockAbilitySO.EnemyLayerMask);
            foreach(var collision in attackable)
            {
                if (collision.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.TakeDamage(rockAbilitySO.SmashDamage, gameObject.transform.position);
                    enemy.Stun(rockAbilitySO.StunDuration);
                }
                if (collision.TryGetComponent<BreakOnSmash>(out var d))
                {
                    d.TakeDamage(rockAbilitySO.SmashDamage, gameObject);
                }
            }
                

            player.ImpulseCamera();
        }

        public void DeactivateAndDestroy()
        {
            if (activeRockFormCoroutine != null)
            {
                StopCoroutine(activeRockFormCoroutine);
                activeRockFormCoroutine = null;
            }
            if (velocityCoroutine != null)
            {
                StopCoroutine(velocityCoroutine);
                velocityCoroutine = null;
            }
            waitForLanding = false;

            StartCoroutine(RevertAndCleanup());
        }

        IEnumerator RevertAndCleanup()
        {
            // Phase 3: Play Transform-Out Animation (using the dedicated TransformOutClip)
            animator.speed = 1f; // Ensure animator speed is normal for playing the out clip

            if (rockAbilitySO.TransformOutClip && transformOutClipLength > 0)
            {
                animator.CrossFadeInFixedTime(rockAbilitySO.TransformOutClip.name, 0f);
                yield return new WaitForSeconds(transformOutClipLength);
            }

            // Phase 4: Cleanup and Revert to Normal
            animator.speed = 1f; // Ensure it's reset if it was ever changed elsewhere (though should be 1)

            animator.CrossFadeInFixedTime("Ado_idle", 0);

            movement.canMove = true;
            combat.canAttack = true;
            if (player.IsInvincible != origInvincible)
                player.ChangeInvincibleState(false);

            if (rockAbilitySO.cooldownTime > 0 && player)
            {
                rockAbilitySO.TriggerCooldownRequestFromEffect(player);
            }
            
            Destroy(this);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (rockAbilitySO == null) return; 
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector3 c = transform.position + (Vector3)rockAbilitySO.SmashAreaOffset;
            Gizmos.DrawWireCube(c, rockAbilitySO.SmashAreaSize);
        }
#endif
    }
}