using EnemyAI;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Healing State")]
public class HealingStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Healing Settings")]
    public float healRate = 1f; // How much health to heal per second
    public float healDuration = 5f; // How long to heal for (or until full health)
    
    [Header("Healing Interruption")]
    public float interruptHealingRange = 7f; // Or use enemy.detectionRange
    private float healProgressAccumulator;

    
    [Header("Transitions")]
    public EnemyStateSO patrolState; // State to go to after healing
    public EnemyStateSO chaseState;  // State to go to if player is detected during healing

    private float healStartTime;

    public override void OnEnter(EnemyController enemy)
    {
        Debug.Log("Entering Healing State");
        enemy.animator.SetBool(isWalking, false); // Stop movement
        // Optionally play a healing animation
        healStartTime = Time.time;
        healProgressAccumulator = 0f; // Reset accumulator

    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (distanceToPlayer <= enemy.fleeDistance && enemy.currentHits > 1)
        {
            enemy.StateMachine.ChangeState(enemy, chaseState);
            return;
        }
        if (Time.time >= healStartTime + healDuration || enemy.currentHits >= enemy.maxHits)
        {
            enemy.currentHits = enemy.maxHits;
            enemy.canFlee = true;
            enemy.StateMachine.ChangeState(enemy, patrolState);
            return;
        }
        healProgressAccumulator += healRate * Time.deltaTime;
        if (healProgressAccumulator >= 1.0f)
        {
            int amountToHeal = Mathf.FloorToInt(healProgressAccumulator);
            enemy.currentHits = Mathf.Min(enemy.maxHits, enemy.currentHits + amountToHeal);
            healProgressAccumulator -= amountToHeal;
        }

        enemy.canFlee = true;
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        // No movement during healing
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
    }

    public override void OnExit(EnemyController enemy)
    {
        Debug.Log("Exiting Healing State");
    }
}