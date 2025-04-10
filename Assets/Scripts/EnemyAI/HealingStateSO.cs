using EnemyAI;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Healing State")]
public class HealingStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Healing Settings")]
    public float healRate = 1f; // How much health to heal per second
    public float healDuration = 5f; // How long to heal for (or until full health)

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
    }

    public override void OnUpdate(EnemyController enemy)
    {
        // Check if player is detected during healing
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (distanceToPlayer <= enemy.detectionRange)
        {
            enemy.StateMachine.ChangeState(enemy, chaseState);
            return;
        }

        // Heal over time
        if (Time.time >= healStartTime + healDuration)
        {
            enemy.currentHits = enemy.maxHits; // Heal to full health
            enemy.StateMachine.ChangeState(enemy, patrolState);
        }
        else
        {
            enemy.currentHits = Mathf.Min(enemy.maxHits, enemy.currentHits + (int)(healRate * Time.deltaTime));
            // Optional: Add visual feedback for healing (e.g., a particle effect)
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