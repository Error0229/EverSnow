using UnityEngine;
using Wrapper;

namespace Enemy.enemyState
{
    public class ChaseState : IEnemyState
    {
        public void OnEnter(Enemy enemy)
        {
            // animator.Play("Chase");
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Sprint);
            enemy.SetDestination(enemy.targetEnemy.transform.position);
        }

        public void OnExit(Enemy enemy)
        {
            enemy.weaponCollider.enabled = false;
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Run);
        }

        public void OnUpdate(Enemy enemy)
        {
            enemy.SetDestination(enemy.targetEnemy.transform.position);
            var enemyState = Enemy.EnemyState.Chase;
            if (Vector3.Distance(enemy.transform.position, enemy.targetEnemy.transform.position) <
                enemy.GetAttackRange())
            {
                var targetPosition =
                    enemy.transform.position +
                    (enemy.targetEnemy.transform.position - enemy.transform.position) * 2;
                enemy.targetPosition = targetPosition;
                enemyState = Enemy.EnemyState.Attack;
            } else if (Vector3.Distance(enemy.transform.position, enemy.targetEnemy.transform.position) > 4f &&
                !enemy.navMeshAgentWrapper.Visible(enemy.targetEnemy.gameObject))
            {
                enemyState = Enemy.EnemyState.Idle;
            }

            enemy.SetState(enemyState);
        }
    }
}