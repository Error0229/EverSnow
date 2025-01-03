using System.Collections.Generic;
using UnityEngine;
using Wrapper;

namespace Enemy.enemyState
{
    public class IdleState : IEnemyState
    {
        private float runtime = 0;
        private float maxRuntime = 3;

        public void OnEnter(Enemy enemy)
        {
            enemy.navMeshAgentWrapper.SetDestination(enemy.transform.position);
            enemy.SetWeaponColliderEnable(false);
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Walk);
        }

        public void OnExit(Enemy enemy)
        {
        }

        float triggerRange = 20f;

        public void OnUpdate(Enemy enemy)
        {
            var t = Time.time;
            var height = 0.5f;
            var view = enemy.GetView();
            var enemyHateList = enemy.GetHateList();
            view.transform.localPosition = new Vector3(0,
                Mathf.Lerp(view.transform.localPosition.y, Mathf.Sin(t) * height, 0.5f), 0);
            var nextEnemyState = Enemy.EnemyState.Idle;
            if (enemy.IsArrived() || runtime > maxRuntime)
            {
                var randomDirection = UnityEngine.Random.Range(0, 360);
                var randomRange = UnityEngine.Random.Range(1, 2);
                var randomPosition = 
                    enemy.transform.position + 
                    new Vector3(Mathf.Cos(randomDirection) * randomRange, 0,
                    Mathf.Sin(randomDirection) * randomRange);
                enemy.SetDestination(randomPosition);
                runtime = 0;
            }
            else
            {
                runtime += Time.deltaTime;
            }
            if (enemyHateList.Count != 0)
            {
                var closestEnemy = GetClosestEnemy(enemy, enemyHateList);
                if (Vector3.Distance(enemy.transform.position, closestEnemy.transform.position) < triggerRange)
                {
                    nextEnemyState = Enemy.EnemyState.Chase;
                    enemy.SetTargetEnemy(closestEnemy);
                }
            }


            if (nextEnemyState != Enemy.EnemyState.Idle) enemy.SetState(nextEnemyState);
        }

        private EnemyHate GetClosestEnemy(Enemy enemy, List<EnemyHate> enemyHateList)
        {
            var closestEnemy = enemyHateList[0];
            foreach (EnemyHate enemyHate in enemyHateList)
            {
                if (Vector3.Distance(enemy.transform.position, enemyHate.transform.position) <
                    Vector3.Distance(enemy.transform.position, closestEnemy.transform.position))
                {
                    closestEnemy = enemyHate;
                }
            }

            return closestEnemy;
        }
    }
}