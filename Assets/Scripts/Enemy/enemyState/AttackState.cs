using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Wrapper;

namespace Enemy.enemyState
{
    public class AttackState : IEnemyState
    {
        private float attackTime = 0;
        private float attackMaxTime = 2;

        public void OnEnter(Enemy enemy)
        {
            enemy.navMeshAgentWrapper.SetDestination(enemy.targetPosition);
            attackTime = 0;
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
            var view = enemy.GetView();
          view.transform.localPosition = new Vector3(0,
                Mathf.Lerp(view.transform.position.y, 0, 0.5f), 0);
            var enemyState = Enemy.EnemyState.Attack;
            if (attackTime >= attackMaxTime || enemy.navMeshAgentWrapper.IsArrived() || enemy.weapon.IsHited)
            {
                attackTime += Time.deltaTime;
                enemyState = Enemy.EnemyState.Away;
            }

            enemy.SetState(enemyState);
        }
    }
}