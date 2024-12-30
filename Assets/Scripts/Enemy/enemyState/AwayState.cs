using UnityEngine;
using Wrapper;

namespace Enemy.enemyState
{
    public class AwayState : IEnemyState
    {
        public void OnEnter(Enemy enemy)
        {
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Run);
            var leaveRange = 10f;
            var hatePosition = enemy.targetEnemy.transform.position;
            var transform = enemy.transform;
            var position = transform.position;
            enemy.navMeshAgentWrapper.SetDestination(position +
                                                     (position - hatePosition).normalized * leaveRange);
        }

        public void OnExit(Enemy enemy)
        {
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Walk);
        }

        public void OnUpdate(Enemy enemy)
        {
            var enemyState = Enemy.EnemyState.Away;
            if (enemy.navMeshAgentWrapper.IsArrived())
            {
                enemyState = Enemy.EnemyState.Idle;
            }

            if(enemyState != Enemy.EnemyState.Away) enemy.SetState(enemyState);
        }
    }
}