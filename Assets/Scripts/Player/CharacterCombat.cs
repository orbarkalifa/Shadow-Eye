using System.Collections;
using Cinemachine;
using EnemyAI;
using Suits;
using Suits.Duri;
using UnityEngine;

namespace Player
{
    public class CharacterCombat : MonoBehaviour
    {
        private static readonly int attackSpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");

        [Header("Attack Settings")]
        public BoxCollider2D attackBox;
        public int attackDamage = 1;
        public LayerMask attackableLayerMask;
        private readonly float basicAttackCooldown = 0.5f;
        public float attackCooldown;
        public bool canAttack;

        private PlayerController character;
        private Animator animator;
        private Rigidbody2D rb;
    
        private float currentSuitAttackAnimSpeed = 1.0f;
        private int comboStep;
        private float lastAttackTime;
        private readonly float comboResetTime = 1f;
        private bool isAttacking;
        private bool isOnCooldown;
        private CharacterMovement characterMovement; 
        private CinemachineImpulseSource impulseSource;
        private float basicRange;
        private bool attackUp;
    
        private void Start()
        {
            canAttack = true;
            character = GetComponent<PlayerController>();
            animator = character.animator;
            if (!animator)
                Debug.LogError("Animator is missing!");
       
            rb = character.rb;
            if (!rb)
                Debug.LogError("Rigidbody2D is missing!");
       
            characterMovement = GetComponent<CharacterMovement>(); 
            if (characterMovement == null)
            {
                Debug.LogError("CharacterMovement component not found on " + gameObject.name);
            }
        
            impulseSource = GetComponent<CinemachineImpulseSource>();
            if (impulseSource == null)
            {
                Debug.LogError("impulse component not found on Main Camera!");
            }

            attackCooldown = basicAttackCooldown;
            basicRange = attackBox.size.x;

        }

        public void PressedUp(bool isPressed)
        {
            attackUp = isPressed;
        }
    
        public void BasicAttack()
        {
            if (isOnCooldown)
                return;
        
            animator.SetFloat(attackSpeedMultiplier, currentSuitAttackAnimSpeed);
        
            if (attackUp)
            {
                isAttacking = true;
                animator.CrossFadeInFixedTime("Ado_upAttack", 0.05f);
                StartCoroutine(AttackCooldown());
                return;
            }
            if (Time.time - lastAttackTime > comboResetTime)
            {
                comboStep = 0;
                isAttacking = false;
            }

            lastAttackTime = Time.time;

            if (!isAttacking)
            {
                comboStep = 1;
                isAttacking = true;
                animator.CrossFadeInFixedTime("Ado_attack1", 0.05f);
            
            }
            else if (comboStep == 1)
            {
                comboStep = 2;
                animator.CrossFadeInFixedTime("Ado_attack2", 0.05f);
            }
            else
            {
                //fallback
                comboStep = 1;
                isAttacking = true;
                animator.CrossFadeInFixedTime("Ado_attack1", 0.05f);
            }
        }

        public void OnAttack1Complete()
        {
            DoAttackHit();
        }

        public void OnAttack2Complete()
        {
            DoAttackHit();
            StartCoroutine(AttackCooldown());
            isAttacking = false;
            comboStep = 0;
        }
    
        public void OnHeavySmashImpact()
        {
            if (character.equippedSuit?.specialAttack is HeavySmashAbility heavySmash)
            {
                heavySmash.ApplySmashEffect(character); // Pass PlayerController
            }
        }
        private IEnumerator AttackCooldown()
        {
            isOnCooldown = true;
            yield return new WaitForSeconds(attackCooldown);
            isOnCooldown = false;
        }

        private void DoAttackHit()
        {
            // decide rotation and size
            float angle = attackUp ? 90f : 0f;
            Vector2 size  = attackUp
                ? new Vector2( attackBox.size.y, attackBox.size.x )   // swap width/height
                : attackBox.size;

            // decide world-space center
            Vector2 center = attackUp
                ? (Vector2)transform.position + Vector2.up * (size.y/2f) 
                : attackBox.transform.position;

            Collider2D[] gotHit = Physics2D.OverlapBoxAll(
                center,
                size,
                angle,
                attackableLayerMask
            );

            float recoilDirection = 0;
            bool hitSomething = false;
            foreach(Collider2D hit in gotHit)
            {
                if (hit.TryGetComponent(out Enemy enemyComponent))
                {
                    recoilDirection = ((Vector2)enemyComponent.transform.position - (Vector2)transform.position)
                        .normalized.x;
                    enemyComponent.TakeDamage(attackDamage, recoilDirection);
                    hitSomething = true;
                }
                else if (hit.TryGetComponent(out Destructible obj))
                {
                    obj.TakeDamage(attackDamage);
                    hitSomething = true;
                }
            }

            if (!hitSomething) return;
            characterMovement.AddRecoil(recoilDirection * -1);
            ApplyRecoil();

        }

        private void ApplyRecoil()
        {
            if (characterMovement != null)
            {
                impulseSource.GenerateImpulse();
            }
            else
            {
                Debug.LogError(impulseSource == null
                    ? "CameraShake reference is missing in CharacterCombat, cannot apply camera shake."
                    : "CharacterMovement component is missing in CharacterCombat, cannot apply recoil.");
            }
        }

        private void ChangeRange(float range)
        {
            attackBox.size = range <= 0f ? new Vector2(basicRange, attackBox.size.y) : new Vector2(range, attackBox.size.y);
        }

        private void ChangeCooldown(float cooldown)
        {
            attackCooldown = cooldown <= 0f ? basicAttackCooldown : cooldown;
        }

        public void ParametersSwap(Suit suit)
        {
            if(suit != null)
            {
                ChangeCooldown(suit.basicAttackCooldown);
                ChangeRange(suit.attackRange);
                currentSuitAttackAnimSpeed = suit.basicAttackAnimationSpeed;
                return;
            }
            ChangeCooldown(-1);
            ChangeRange(-1);
        }
    
#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (attackBox == null)
                return;

            // calculate the same angle, size and center you use at runtime
            var angle = attackUp ? 90f : 0f;
            var size  = attackUp
                ? new Vector2(attackBox.size.y, attackBox.size.x)  // swap w/h
                : attackBox.size;
            var center = attackUp
                ? (Vector2)transform.position + Vector2.up * (size.y * 0.5f)
                : (Vector2)attackBox.transform.position;

            Gizmos.color = Color.red;
            Matrix4x4 oldMat = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = oldMat;
        }
#endif
    
    }
}
