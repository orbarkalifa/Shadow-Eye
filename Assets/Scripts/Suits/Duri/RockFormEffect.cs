using System.Collections;
using Cinemachine;
using EnemyAI;
using UnityEngine;
namespace Suits.Duri
{
    public class RockFormEffect : MonoBehaviour
    {
        
        RockAbility cfg;
        MainCharacter player;
        CharacterMovement movement;
        CharacterCombat combat;
        Rigidbody2D rb;
        Animator animator;
        CinemachineImpulseSource impulseSource;
        bool origInvincible;
        Coroutine velocityCoroutine;
        bool waitForLanding;
        float transformLength;
        
        void Awake()
        {
            player=GetComponent<MainCharacter>();
            movement=GetComponent<CharacterMovement>();
            combat=GetComponent<CharacterCombat>();
            rb=GetComponent<Rigidbody2D>();
            animator=GetComponent<Animator>();
            impulseSource=GetComponent<CinemachineImpulseSource>();
            if (impulseSource==null) 
                Debug.LogWarning("RockFormEffect:No CinemachineImpulseSource–camera shake disabled.");
        }
        
        public void Initialize(RockAbility config)
        {
            cfg=config;
            transformLength=cfg.TransformClip.length;
        }
        
        public void Activate()
        {
            origInvincible=player.IsInvincible;
            movement.canMove=false;
            combat.canAttack=false;
            player.ChangeInvincibleState();
            animator.speed=1f;
            animator.CrossFadeInFixedTime(cfg.TransformClip.name,0f,0,0f);
            if(velocityCoroutine!=null)StopCoroutine(velocityCoroutine);
            velocityCoroutine=StartCoroutine(DropVelocity());
            waitForLanding=!player.IsGrounded();
        }
        
        void Update()
        {
            if (waitForLanding && player.IsGrounded())
            {
                Smash();waitForLanding=false;
            }
        }

        void FixedUpdate()
        {
            if (waitForLanding) rb.velocity+=new Vector2(0,-1f);
        }
        
        IEnumerator DropVelocity()
        {
            while(Mathf.Abs(rb.velocity.x)>0.05f)
            {
                float newX=Mathf.MoveTowards(rb.velocity.x,0f,cfg.VelocityDropRate*Time.deltaTime);
                rb.velocity=new Vector2(newX,rb.velocity.y);
                yield return null;
            }
            rb.velocity=new Vector2(0f,rb.velocity.y);
        }
        
        void Smash()
        {
            Vector2 center=(Vector2)transform.position+cfg.SmashAreaOffset;
            var enemies=Physics2D.OverlapBoxAll(center,cfg.SmashAreaSize,0f,cfg.EnemyLayerMask);
            foreach (var c in enemies) 
                if (c.TryGetComponent<Enemy>(out var e))
                {
                    float dir=Mathf.Sign(e.transform.position.x-transform.position.x);
                    if(dir==0)dir=-1;
                    e.TakeDamage(cfg.SmashDamage,dir);
                    e.Stun(cfg.StunDuration);
                }
            if (impulseSource!=null)
            {
                impulseSource.GenerateImpulse();
                Debug.Log("Rock Smash–Camera Shake Triggered!");
                
            }
        }

        public void DeactivateAndDestroy()
        {
            StartCoroutine(RevertAndCleanup());
        }
        
        IEnumerator RevertAndCleanup()
        {
            animator.speed = -1f;
            animator.CrossFadeInFixedTime(cfg.TransformClip.name,0.1f);
            yield return new WaitForSeconds(transformLength);
            animator.speed = 1f;
            movement.canMove=true;
            combat.canAttack=true;
            if (player.IsInvincible != origInvincible) 
                player.ChangeInvincibleState();
            player.StartCoroutine(player.SpecialMovementCD(cfg.cooldownTime));
            Destroy(this);
        }
    #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if(cfg==null)return;
            Gizmos.color=new Color(1,0,0,0.5f);
            Vector3 c=transform.position+(Vector3)cfg.SmashAreaOffset;
            Gizmos.DrawWireCube(c,cfg.SmashAreaSize);
        }
    #endif
    }
}
