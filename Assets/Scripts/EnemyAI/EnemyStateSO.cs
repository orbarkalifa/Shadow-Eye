using UnityEngine;

/// <summary>
/// Base class for all enemy states implemented as ScriptableObjects.
/// </summary>
public abstract class EnemyStateSO : ScriptableObject
{
    /// <summary>
    /// Called once when this state first becomes active.
    /// </summary>
    public abstract void OnEnter(EnemyController enemy);

    /// <summary>
    /// Called every frame (Update).
    /// </summary>
    public abstract void OnUpdate(EnemyController enemy);

    /// <summary>
    /// Called every physics frame (FixedUpdate).
    /// </summary>
    public abstract void OnFixedUpdate(EnemyController enemy);

    /// <summary>
    /// Called once when transitioning away from this state.
    /// </summary>
    public abstract void OnExit(EnemyController enemy);
}