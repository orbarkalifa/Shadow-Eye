using EnemyAI;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/States/Healing State")]
public class HealingStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Healing Settings")]
    public float healRate = 1f; // How much health to heal per second
    [Header("Healing Interruption")]
    private float healProgressAccumulator;

    


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
        base.OnUpdate(enemy);
        healProgressAccumulator += healRate * Time.deltaTime;
        if (healProgressAccumulator >= 1.0f)
        {
            int amountToHeal = Mathf.FloorToInt(healProgressAccumulator);
            enemy.currentHits = Mathf.Min(enemy.maxHits, enemy.currentHits + amountToHeal);
            healProgressAccumulator -= amountToHeal;
        }

        
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

    protected override bool Eval(EnemyController enemy)
    {
        if(enemy.currentHits>= enemy.maxHits) return false;
        return enemy.GetDistanceToPlayer()>enemy.fleeDistance;
    }
}