using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Wrapper;

namespace Enemy.enemyState
{
    public class RabbitAttackState : IEnemyState
    {
        private float attackTime = 0;
        private float attackMaxTime = 0.2f;
        private GameObject debugFlag;

        public void OnEnter(Enemy enemy)
        {
            // enemy.animator.Play("Jump");
            var targetPosition = enemy.transform.position +
                                 (enemy.targetEnemy.transform.position - enemy.transform.position) * 2;
            enemy.navMeshAgentWrapper.SetDestination(targetPosition);
            GameObject prefab = Resources.Load<GameObject>("char");
            debugFlag = GameObject.Instantiate(prefab);
            debugFlag.transform.position = targetPosition;

            var animationFullTime = enemy.animator.GetCurrentAnimatorStateInfo(0).length;
            var speed = animationFullTime * (1 / attackMaxTime);
            enemy.navMeshAgentWrapper.SetSpeed(speed);
            attackTime = 0;
            enemy.weaponCollider.enabled = true;
            enemy.weaponCollider.isTrigger = true;
            enemy.weapon.Reset();

            var parti = enemy.GetComponentInChildren<ParticleSystem>();
            if (parti)
            {
                parti.Stop();
                parti.Play();
            }
        }

        public void OnExit(Enemy enemy)
        {
            GameObject.Destroy(debugFlag);
            enemy.weaponCollider.enabled = false;
            enemy.navMeshAgentWrapper.SetSpeed(NavMeshAgentWrapper.MoveSpeed.Run);
            enemy.animator.Play("Idle");
            var parti = enemy.GetComponentInChildren<ParticleSystem>();
            if (parti)
            {
                parti.Stop();
            }
        }

        public void OnUpdate(Enemy enemy)
        {
            var view = enemy.GetView();
            view.transform.localPosition = new Vector3(0,
                Mathf.Lerp(view.transform.localPosition.y, 0, 0.5f), 0);
            var enemyState = Enemy.EnemyState.Attack;
            if (attackTime >= attackMaxTime || enemy.navMeshAgentWrapper.IsArrived() || enemy.weapon.IsHited)
            {
                attackTime += Time.deltaTime;
                enemyState = Enemy.EnemyState.Away;
            }

            if (enemyState != Enemy.EnemyState.Attack) enemy.SetState(enemyState);
        }
    }
}