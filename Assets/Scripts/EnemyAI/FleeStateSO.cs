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


    public override void OnFixedUpdate(EnemyController enemy)
    {
      enemy.Flee();
    }

    public override void OnExit(EnemyController enemy)
    {
        Debug.Log("Exiting Flee State");
    }

    protected override bool Eval(EnemyController enemy)
    {
        return enemy.currentHits <= 1 && enemy.canFlee;
    }
}