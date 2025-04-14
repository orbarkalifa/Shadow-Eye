using EnemyAI;
using UnityEngine;


[CreateAssetMenu(menuName = "EnemyAI/States/Flee State")]
public class FleeStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    [Header("Flee Settings")]
    public float fleeSpeed = 6f;
    public float fleeDistance = 10f;

    [Header("Transitions")]
    public EnemyStateSO healingState;
    public EnemyStateSO chaseState;

    public override void OnEnter(EnemyController enemy)
    {
        Debug.Log("Entering Flee State");
        enemy.animator.SetBool(isWalking, true);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if(Vector2.Distance(enemy.transform.position, enemy.player.position) > fleeDistance)
        {
            enemy.StateMachine.ChangeState(enemy, healingState); // Transition to healing
        }
        if (enemy.IsDeadEnd() || Vector2.Distance(enemy.transform.position, enemy.homePosition.position) > enemy.maxChaseDistance)
        {
            enemy.canFlee = false;
            enemy.StateMachine.ChangeState(enemy, chaseState);
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
        Vector2 directionToPlayer = enemy.player.position - enemy.transform.position;
        Vector2 fleeDirection = -directionToPlayer.normalized;

        enemy.rb.velocity = new Vector2(fleeDirection.x * fleeSpeed, enemy.rb.velocity.y);
        enemy.UpdateFacingDirection(fleeDirection.x); 
    }

    public override void OnExit(EnemyController enemy)
    {
        Debug.Log("Exiting Flee State");
    }

    
}