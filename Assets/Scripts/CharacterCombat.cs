using System.Collections;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackRange;
    public int attackDamage = 1;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.3f;
    [Header("Recoil Settings")]
    [SerializeField] private float recoilForce = 5f;

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
    
    public void BasicAttack(Vector2 facingDirection)
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

        bool enemyHit = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemyController enemyComponent))
            {
                enemyComponent.TakeDamage(attackDamage);
                enemyHit = true;
            }
        }

        if (enemyHit)
        {
            ApplyRecoil();
        }
    }

    private void ApplyRecoil()
    {
        if (characterMovement != null)
        {
            Vector2 recoilDirection = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
            characterMovement.GetComponent<Rigidbody2D>().AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);
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
