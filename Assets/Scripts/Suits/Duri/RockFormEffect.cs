using System.Collections;
using Cinemachine;
using EnemyAI;
using Player;
using UnityEngine;

namespace Suits.Duri
{
    public class RockFormEffect : MonoBehaviour
    {
        RockAbility cfg; // This will hold the reference to your RockAbility ScriptableObject
        MainCharacter player;
        CharacterMovement movement;
        CharacterCombat combat;
        Rigidbody2D rb;
        Animator animator;
        CinemachineImpulseSource impulseSource;

        bool origInvincible;
        Coroutine activeRockFormCoroutine;
        Coroutine velocityCoroutine;
        bool waitForLanding;
        float transformInClipLength;
        float transformOutClipLength; 

        void Awake()
        {
            player = GetComponent<MainCharacter>();
            movement = GetComponent<CharacterMovement>();
            combat = GetComponent<CharacterCombat>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            impulseSource = GetComponent<CinemachineImpulseSource>();

            if (animator == null) Debug.LogError("RockFormEffect: Animator not found!");
            if (impulseSource == null)
                Debug.LogWarning("RockFormEffect: No CinemachineImpulseSource – camera shake disabled.");
        }

        public void Initialize(RockAbility config) 
        {
            cfg = config; 

            if (cfg.TransformInClip == null)
            {
                Debug.LogError("RockFormEffect: TransformInClip is not assigned in RockAbility config!", cfg);
                transformInClipLength = 0f;
                enabled = false; 
                return;
            }
            transformInClipLength = cfg.TransformInClip.length;

            if (cfg.TransformOutClip == null)
            {
                Debug.LogWarning("RockFormEffect: TransformOutClip is not assigned. Reverting animation will be skipped.", cfg);
                transformOutClipLength = 0f;
            }
            else
            {
                transformOutClipLength = cfg.TransformOutClip.length;
            }
        }

        public void Activate()
        {
            if (!enabled) return; 

            origInvincible = player.IsInvincible;
            movement.canMove = false;
            combat.canAttack = false;
            player.ChangeInvincibleState();

            if (velocityCoroutine != null) StopCoroutine(velocityCoroutine);
            velocityCoroutine = StartCoroutine(DropVelocity());

            if (activeRockFormCoroutine != null) StopCoroutine(activeRockFormCoroutine); // Stop if somehow called again
            activeRockFormCoroutine = StartCoroutine(ManageRockFormAnimation());
        }

        IEnumerator ManageRockFormAnimation()
        {
            // Phase 1: Play Transform-In Animation
            animator.speed = 1f;
            animator.CrossFadeInFixedTime(cfg.TransformInClip.name, 0f);
            yield return new WaitForSeconds(transformInClipLength+1);

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
                rb.velocity += Vector2.down * (cfg.VelocityDropRate * 0.1f * Time.fixedDeltaTime);
            }
        }

        IEnumerator DropVelocity()
        {
            while (Mathf.Abs(rb.velocity.x) > 0.05f)
            {
                float newX = Mathf.MoveTowards(rb.velocity.x, 0f, cfg.VelocityDropRate * Time.deltaTime);
                rb.velocity = new Vector2(newX, rb.velocity.y);
                yield return null;
            }
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        void Smash()
        {
            Vector2 center = (Vector2)transform.position + cfg.SmashAreaOffset;
            var enemies = Physics2D.OverlapBoxAll(center, cfg.SmashAreaSize, 0f, cfg.EnemyLayerMask);
            foreach (var c in enemies)
                if (c.TryGetComponent<Enemy>(out var e))
                {
                    float dir = Mathf.Sign(e.transform.position.x - transform.position.x);
                    if (dir == 0) dir = -1f;
                    e.TakeDamage(cfg.SmashDamage, dir);
                    e.Stun(cfg.StunDuration);
                }
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }
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

            if (cfg.TransformOutClip != null && transformOutClipLength > 0)
            {
                animator.CrossFadeInFixedTime(cfg.TransformOutClip.name, 0f);
                yield return new WaitForSeconds(transformOutClipLength);
            }

            // Phase 4: Cleanup and Revert to Normal
            animator.speed = 1f; // Ensure it's reset if it was ever changed elsewhere (though should be 1)

            animator.CrossFadeInFixedTime("Ado_idle", 0);

            movement.canMove = true;
            combat.canAttack = true;
            if (player.IsInvincible != origInvincible)
                player.ChangeInvincibleState();

            if (cfg.cooldownTime > 0)
            {
                player.StartCoroutine(player.SpecialMovementCD(cfg.cooldownTime));
            }

            Destroy(this);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (cfg == null) return; 
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector3 c = transform.position + (Vector3)cfg.SmashAreaOffset;
            Gizmos.DrawWireCube(c, cfg.SmashAreaSize);
        }
#endif
    }
}