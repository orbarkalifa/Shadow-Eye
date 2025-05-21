using System.Collections;
using Cinemachine;
using EnemyAI;
using UnityEngine;

namespace Suits.Abilities
{
    public class RockFormEffect : MonoBehaviour
{
    RockAbility cfg;
    MainCharacter player;
    CharacterMovement movement;
    CharacterCombat combat;
    Rigidbody2D rb;
    Animator animator;
    CinemachineImpulseSource impulseSource; // Add this
    
    bool origInvincible;
    Coroutine velocityCoroutine;
    private bool waitForLanding;

    private void Awake()
    {
        player = GetComponent<MainCharacter>();
        movement = GetComponent<CharacterMovement>();
        combat = GetComponent<CharacterCombat>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>(); // Get the component

        if (impulseSource == null)
        {
            Debug.LogWarning("RockFormEffect: CinemachineImpulseSource not found on the player. Camera shake on smash will not work.");
        }
    }

    public void Initialize(RockAbility config)
    {
        cfg = config;
    }

    public void Activate()
    {
        origInvincible = player.IsInvincible; 
        movement.canMove = false;
        combat.canAttack = false;
        
        player.ChangeSprite(cfg.RockSprite);
        player.ChangeInvincibleState();

        movement.canMove = false;
        combat.canAttack = false;

        if (velocityCoroutine != null) StopCoroutine(velocityCoroutine);
        velocityCoroutine = StartCoroutine(DropVelocity());

        waitForLanding = !player.IsGrounded();
        
    }

    void Update()
    {
        if (waitForLanding && player.IsGrounded())
        {
            Smash();
            waitForLanding = false;
        }
    }

    private void FixedUpdate()
    {
        if (waitForLanding)
        {
            rb.velocity += new Vector2(0,-1);
        }
    }

    IEnumerator DropVelocity()
    {
        while (Mathf.Abs(rb.velocity.x) > 0.05f)
        {
            float newX = Mathf.MoveTowards(
                rb.velocity.x,
                0f,
                cfg.VelocityDropRate * Time.deltaTime
            );
            rb.velocity = new Vector2(newX, rb.velocity.y);
            yield return null;
        }
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void Smash()
    {
        Vector2 center = (Vector2)transform.position + cfg.SmashAreaOffset;

        var enemies = Physics2D.OverlapBoxAll(
            center, cfg.SmashAreaSize, 0f, cfg.EnemyLayerMask);
        foreach (var c in enemies)
            if (c.TryGetComponent(out Enemy e))
            {
                // Determine a recoil direction based on relative position for this specific enemy
                float recoilDirForEnemy = Mathf.Sign(e.transform.position.x - transform.position.x);
                if (recoilDirForEnemy == 0) recoilDirForEnemy = -1; // Default if perfectly aligned vertically
                e.TakeDamage(cfg.SmashDamage, recoilDirForEnemy); // Pass the calculated direction
                e.Stun(cfg.StunDuration);
            }

        // Trigger Camera Shake
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
            Debug.Log("Rock Smash - Camera Shake Triggered!");
        }
        else
        {
            Debug.LogWarning("Rock Smash - No Impulse Source to trigger shake.");
        }
    }

    public void DeactivateAndDestroy()
    {
        movement.canMove = true;
        combat.canAttack = true;
        
        if (animator != null) animator.enabled = true;

        if (player.IsInvincible != origInvincible)
            player.ChangeInvincibleState();

        player.StartCoroutine(player.SpecialMovementCD(cfg.cooldownTime));
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
