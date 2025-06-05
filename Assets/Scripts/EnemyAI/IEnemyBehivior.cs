namespace EnemyAI
{
    public interface IEnemyBehivior
    {
        public abstract void Attack();
        public abstract void Patrol();
        //public void Idle();
        public abstract void Chase();
        public abstract void Flee();
        public abstract void ReturnHome();
    }
}