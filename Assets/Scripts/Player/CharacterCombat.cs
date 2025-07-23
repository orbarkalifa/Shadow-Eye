using System.Collections;
using Cinemachine;
using EnemyAI;
using Suits;
using Suits.Duri;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

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
                StartCoroutine(AttackCooldown());
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
            Collider2D[] gotHit = Physics2D.OverlapBoxAll(
                attackBox.transform.position,
                attackBox.size,
                attackBox.transform.eulerAngles.z,
                attackableLayerMask
            );

            Vector2 recoilDirection = Vector2.zero;
            bool hitSomething = false;
            foreach(Collider2D hit in gotHit)
            {
                if (hit.TryGetComponent(out Enemy enemyComponent))
                {
                    recoilDirection = ((Vector2)transform.position - (Vector2)enemyComponent.transform.position)
                        .normalized;
                    enemyComponent.TakeDamage(attackDamage, transform.position);
                    hitSomething = true;
                }
                else if (hit.TryGetComponent(out Destructible obj))
                {
                    obj.TakeDamage(attackDamage);
                    hitSomething = true;
                }
            }

            if (!hitSomething) return;
            characterMovement.AddRecoil(recoilDirection);
            character.ImpulseCamera();

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
    }
}
