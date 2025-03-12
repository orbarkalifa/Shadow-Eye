using UnityEngine;

public abstract class EnemyStateSO : ScriptableObject
{
    public abstract void OnEnter(EnemyController enemy);
    public abstract void OnUpdate(EnemyController enemy);
    public abstract void OnFixedUpdate(EnemyController enemy);
    public abstract void OnExit(EnemyController enemy);
}