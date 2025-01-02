using UnityEngine;
using Wrapper;

namespace Enemy.enemyState
{
    public class AwayState : IEnemyState
    {
        float awayTime = 0;
        float awayMaxTime = 4;
        public void OnEnter(Enemy enemy)
        {
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Run);
            var leaveRange = 10f;
            var hatePosition = enemy.targetEnemy.transform.position;
            var transform = enemy.transform;
            var position = transform.position;
            enemy.navMeshAgentWrapper.SetDestination(position +
                                                     (position - hatePosition).normalized * leaveRange);
            awayTime = 0;
        }

        public void OnExit(Enemy enemy)
        {
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Walk);
        }

        public void OnUpdate(Enemy enemy)
        {
            var view = enemy.GetView();
            view.transform.localPosition = new Vector3(0,
                Mathf.Lerp(view.transform.localPosition.y, 0, 0.9f), 0);
            var enemyState = Enemy.EnemyState.Away;
            if (enemy.navMeshAgentWrapper.IsArrived() || awayTime > awayMaxTime)
            {
                enemyState = Enemy.EnemyState.Idle;
            }
            awayTime += Time.deltaTime;

            if(enemyState != Enemy.EnemyState.Away) enemy.SetState(enemyState);
        }
    }
}