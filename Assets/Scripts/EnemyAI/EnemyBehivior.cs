namespace EnemyAI
{
    public interface IEnemyBehivior
    {
        public void Attack();
        public void Patrol();
        //public void Idle();
        public void Chase();
        public void Flee();
        public void ReturnHome();
    }
}