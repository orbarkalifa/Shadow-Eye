using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

public abstract class EnemyStateSO : ScriptableObject
{
    [SerializeField]protected List<EnemyStateSO> States;
    public abstract void OnEnter(EnemyController enemy);

    public virtual void OnUpdate(EnemyController enemy)
    {

        foreach(var state in States)
        {
            if(state)
            {
                if(state.Eval(enemy))
                {
                    enemy.StateMachine.ChangeState(enemy,state);
                    return;                    
                }
                
            }
        }   
    }
    public abstract void OnFixedUpdate(EnemyController enemy);
    public abstract void OnExit(EnemyController enemy);

    protected abstract bool Eval(EnemyController enemy);
}