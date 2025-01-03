using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Wrapper;

namespace Enemy.enemyState
{
    public class AttackState : IEnemyState
    {
        private float attackTime = 0;
        private float attackMaxTime = 2;

        public virtual void OnEnter(Enemy enemy)
        {
            enemy.navMeshAgentWrapper.SetDestination(enemy.targetPosition);
            attackTime = 0;
            enemy.weaponCollider.enabled = true;
            enemy.weaponCollider.isTrigger = true;
            enemy.weapon.Reset();
            enemy.SetSfxMaxTime(1f);
        }

        public virtual void OnExit(Enemy enemy)
        {
            enemy.weaponCollider.enabled = false;
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Run);
            enemy.ResetSfxMaxTime();
        }

        public virtual void OnUpdate(Enemy enemy)
        {
            var view = enemy.GetView();
            view.transform.localPosition = new Vector3(0,
                enemy.GetDefaultHeight() + Mathf.Lerp(view.transform.localPosition.y, 0, 0.9f), 0);
            var enemyState = Enemy.EnemyState.Attack;
            attackTime += Time.deltaTime;
            if (attackTime >= attackMaxTime || enemy.navMeshAgentWrapper.IsArrived() || enemy.weapon.IsHited)
            {
                enemyState = Enemy.EnemyState.Away;
            }

            if (enemyState != Enemy.EnemyState.Attack) enemy.SetState(enemyState);
        }
    }
}