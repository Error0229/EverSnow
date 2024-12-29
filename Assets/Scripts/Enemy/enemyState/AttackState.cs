using Wrapper;

namespace Enemy.enemyState
{
    public class AttackState : IEnemyState
    {
        public void OnEnter(Enemy enemy)
        {
            enemy.navMeshAgentWrapper.SetDestination(enemy.transform.position);
            enemy.weaponCollider.enabled = true;
            enemy.weaponCollider.isTrigger = true;
            enemy.weapon.Reset();
        }

        public void OnExit(Enemy enemy)
        {
            enemy.weaponCollider.enabled = false;
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Run);
        }

        public void OnUpdate(Enemy enemy)
        {
            var enemyState = Enemy.EnemyState.Attack;
            if (enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                enemyState = Enemy.EnemyState.Away;
            }

            enemy.SetState(enemyState);
        }
    }
}