    using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "EnemyAI/States/Return Home State")]
    public class ReturnHomeStateSO : EnemyStateSO
    {
        
        public EnemyStateSO patrolState;
        [Header("Arrival")]
        public float homeArrivalThreshold = 1f;

        public override void OnEnter(EnemyController enemy)
        {
            enemy.animator.SetBool("isWalking", true);
        }

        public override void OnUpdate(EnemyController enemy)
        {
            float distToHome = Vector2.Distance(enemy.transform.position, enemy.homePosition);
            if (distToHome <= homeArrivalThreshold)
            {
                enemy.StateMachine.ChangeState(enemy, patrolState);
            }
        }

        public override void OnFixedUpdate(EnemyController enemy)
        {
            enemy.ReturnHome();
        }

        public override void OnExit(EnemyController enemy)
        {
            enemy.rb.velocity = Vector2.zero;
        }
    }

}