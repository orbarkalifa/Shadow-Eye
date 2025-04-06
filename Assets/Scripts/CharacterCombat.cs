using System.Collections;
using EnemyAI;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackRange;
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
    
    [Header("Camera Shake Settings")]
    [SerializeField] private float hitShakeDuration = 0.1f;
    [SerializeField] private float hitShakeMagnitude = 0.05f;

    private CameraShake cameraShake;
    
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterMovement = GetComponent<CharacterMovement>(); 
        if (characterMovement == null)
        {
            Debug.LogError("CharacterMovement component not found on " + gameObject.name);
        }
        
        cameraShake = Camera.main?.GetComponent<CameraShake>();
        if (cameraShake == null)
        {
            Debug.LogError("CameraShake component not found on Main Camera!");
        }
        
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
            attackRange.position,
            attackRange.GetComponent<BoxCollider2D>().size,
            0f,
            enemyLayer
        );
        Vector2 recoilDirection = Vector2.zero;
        bool enemyHit = false;
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out Enemy enemyComponent))
            {
                recoilDirection = ( (Vector2)enemyComponent.transform.position -  (Vector2)transform.position).normalized;
                if(recoilDirection.x !=0)
                    recoilDirection = recoilDirection.x>0 ? Vector2.right : Vector2.left;
                else
                {
                    recoilDirection = Vector2.up;
                }
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
            cameraShake.ShakeCamera(hitShakeDuration, hitShakeMagnitude); 
        }
        else
        {
            if (cameraShake == null)
                Debug.LogError("CameraShake reference is missing in CharacterCombat, cannot apply camera shake.");
            else
                Debug.LogError("CharacterMovement component is missing in CharacterCombat, cannot apply recoil.");
        }
    }
}
