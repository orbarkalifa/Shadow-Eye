using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TransitionBase : MonoBehaviour
{
    [SerializeField] protected GameState sourceState;
    [SerializeField] protected GameState targetState;
    public GameState TargetState => targetState;

    protected virtual void Awake()
    {
        sourceState = GetComponentInParent<GameState>();
        if(sourceState == null )
        {
            Debug.LogError($"[{name}] Unable to find source state in parent.");
        }
        if(targetState == null )
        {
            Debug.LogError($"[{name}] Target state is not assigned in the Inspector.");
        }
    }

    public abstract bool ShouldTransition();
}