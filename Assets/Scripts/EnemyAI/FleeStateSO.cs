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
        enemy.isFleeing = true;
        enemy.animator.SetBool(isWalking, true);
    }


    public override void OnFixedUpdate(EnemyController enemy)
    {
      enemy.Flee();
    }

    public override void OnExit(EnemyController enemy)
    {
        Debug.Log("Exiting Flee State");
        enemy.isFleeing = false;
    }

    protected override bool Eval(EnemyController enemy)
    {
        if (enemy.currentHits > enemy.lowHealthHP) return false;
        if(enemy.canFlee)
        {
            return enemy.GetDistanceToPlayer() < enemy.fleeDistance;
        }
        return false;
    }
}