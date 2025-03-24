using UnityEngine;

namespace EnemyAI
{
    public class EnemyStateMachine
    {
        public EnemyStateSO CurrentState { get; private set; }

        public void Initialize(EnemyController enemy, EnemyStateSO startingState)
        {
            CurrentState = startingState;
            Debug.Log("Enemy state is now: " + CurrentState);
            CurrentState?.OnEnter(enemy);
        }

        public void ChangeState(EnemyController enemy, EnemyStateSO newState)
        {
            Debug.Log($"Changing state from {CurrentState} to {newState}");
            CurrentState?.OnExit(enemy);
            CurrentState = newState;
            CurrentState?.OnEnter(enemy);
        }

        public void Update(EnemyController enemy)
        {
            CurrentState?.OnUpdate(enemy);
        }

        public void FixedUpdate(EnemyController enemy)
        {
            CurrentState?.OnFixedUpdate(enemy);
        }
    }
}