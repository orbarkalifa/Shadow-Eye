using System.Collections;
using EnemyAI;
using UnityEngine;
using Cinemachine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    /*public Transform attackRange;*/
    public BoxCollider2D attackBox;
    public int attackDamage = 1;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.3f;
    [Header("Recoil Settings")]

    private Animator animator;
    private int comboStep;
    private float lastAttackTime;
    private readonly float comboResetTime = 1f;
    private bool isAttacking;
    private bool isOnCooldown;
    private CharacterMovement characterMovement; 
    private CinemachineImpulseSource impulseSource;
    private float basicRange;
    
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterMovement = GetComponent<CharacterMovement>(); 
        if (characterMovement == null)
        {
            Debug.LogError("CharacterMovement component not found on " + gameObject.name);
        }
        
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            Debug.LogError("impuls component not found on Main Camera!");
        }

        basicRange = attackBox.size.x;

    }
    
    public void BasicAttack(int facingDirection)
    {
        if (isOnCooldown)
            return;

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
    

    private IEnumerator AttackCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    private void DoAttackHit()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackBox.transform.position,
            attackBox.size,
            0f,
            enemyLayer
        );
        float recoilDirection = 0;
        bool enemyHit = false;
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out Enemy enemyComponent))
            {
                recoilDirection = ((Vector2)enemyComponent.transform.position -  (Vector2)transform.position).normalized.x;
                enemyComponent.TakeDamage(attackDamage , recoilDirection);
                enemyHit = true;
            }
        }

        if (enemyHit)
        {
            characterMovement.AddRecoil(recoilDirection*-1);
            ApplyRecoil();
        }
    }

    private void ApplyRecoil()
    {
        if (characterMovement != null)
        {
            impulseSource.GenerateImpulse();
        }
        else
        {
            if (impulseSource == null)
                Debug.LogError("CameraShake reference is missing in CharacterCombat, cannot apply camera shake.");
            else
                Debug.LogError("CharacterMovement component is missing in CharacterCombat, cannot apply recoil.");
        }
    }

    public void ChangeRange(float range)
    {
        if(range <= 0f)
            attackBox.size = new Vector2(basicRange, attackBox.size.y);
        else
            attackBox.size = new Vector2(range, attackBox.size.y);
        
    }
}
