using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "EnemyAI/States/Return Home State")]
    public class ReturnHomeStateSO : EnemyStateSO
    {
        public float returnSpeed = 2f;
        public EnemyStateSO patrolState;

        public override void OnEnter(EnemyController enemy)
        {
            enemy.animator.SetBool("isWalking", true);
        }

        public override void OnUpdate(EnemyController enemy)
        {
            float distToHome = Vector2.Distance(enemy.transform.position, enemy.homePosition);
            if (distToHome <= 0.1f)
            {
                enemy.StateMachine.ChangeState(enemy, patrolState);
            }
        }

        public override void OnFixedUpdate(EnemyController enemy)
        {
            Vector2 dir = (enemy.homePosition - (Vector2)enemy.transform.position).normalized;
            enemy.rb.velocity = new Vector2(dir.x * returnSpeed, enemy.rb.velocity.y);
            enemy.UpdateFacingDirection(dir.x);
        }

        public override void OnExit(EnemyController enemy)
        {
            enemy.rb.velocity = Vector2.zero;
        }
    }

}