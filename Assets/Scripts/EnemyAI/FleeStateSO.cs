using EnemyAI;
using UnityEngine;


[CreateAssetMenu(menuName = "EnemyAI/States/Flee State")]
public class FleeStateSO : EnemyStateSO
{
    private static readonly int isWalking = Animator.StringToHash("isWalking");


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
        if(Vector2.Distance(enemy.transform.position, enemy.player.position) > enemy.fleeDistance)
        {
            enemy.StateMachine.ChangeState(enemy, healingState); // Transition to healing
        }
        if (enemy.IsDeadEnd() || Vector2.Distance(enemy.transform.position, enemy.homePosition) > enemy.maxChaseDistance)
        {
            enemy.canFlee = false;
            enemy.StateMachine.ChangeState(enemy, chaseState);
        }
    }

    public override void OnFixedUpdate(EnemyController enemy)
    {
      enemy.Flee();
    }

    public override void OnExit(EnemyController enemy)
    {
        Debug.Log("Exiting Flee State");
    }

    
}