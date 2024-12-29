using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Wrapper;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        enum EnemyState
        {
            Idle,
            Chase,
            Attack
        }

        private GameObject view;
        private EnemyState _enemyState;

        private EnemyState enemyState
        {
            get => _enemyState;
            set
            {
                OnStateExit(_enemyState);
                _enemyState = value;
                OnStateEnter(_enemyState);
            }
        }

        private void OnStateExit(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    break;
                case EnemyState.Chase:
                    break;
                case EnemyState.Attack:
                    weaponCollider.enabled = false;
                    break;
            }
        }

        private void OnStateEnter(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    navMeshAgentWrapper.SetDestination(transform.position);
                    weaponCollider.enabled = false;
                    // animator.Play("Idle");
                    break;
                case EnemyState.Chase:
                    // animator.Play("Chase");
                    break;
                case EnemyState.Attack:
                    navMeshAgentWrapper.SetDestination(transform.position);
                    weaponCollider.enabled = true;
                    weaponCollider.isTrigger = true;
                    weapon.Reset();
                    // animator.Play("Attack");
                    break;
            }
        }

        private List<EnemyHate> enemyHateList = new();
        NavMeshAgentWrapper navMeshAgentWrapper;

        private EnemyHate targetEnemy;

        // private NavMeshAgent navMeshAgent;
        private Animator animator;
        private Collider weaponCollider;
        private Weapon weapon;

        private void Start()
        {
            view = transform.GetChild(0).gameObject;
            animator = GetComponentInChildren<Animator>();
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgentWrapper = new NavMeshAgentWrapper(navMeshAgent);

            EnemyHate[] enemyHate = GameObject.FindObjectsByType<EnemyHate>(FindObjectsSortMode.None);
            enemyHateList = new List<EnemyHate>(enemyHate);

            weapon = GetComponentInChildren<Weapon>();
            weaponCollider = weapon.gameObject.GetComponent<CapsuleCollider>();
            if (weaponCollider == null)
            {
                throw new Exception("Weapon collider not found");
            }

            enemyState = EnemyState.Idle;
        }

        private void Update()
        {
            var attackRange = 1f;
            switch (_enemyState)
            {
                case EnemyState.Idle:
                    var t = Time.time;
                    var height = 1;
                    view.transform.localPosition = new Vector3(0,
                        Mathf.Lerp(view.transform.position.y, Mathf.Sin(t) * height, 0.5f), 0);
                    if (enemyHateList.Count != 0)
                    {
                        var closestEnemy = GetClosestEnemy();
                        var triggerRange = 4f;
                        var speed = 10f;

                        if (Vector3.Distance(transform.position, closestEnemy.transform.position) < attackRange)
                        {
                            enemyState = EnemyState.Attack;
                        }
                        else if (Vector3.Distance(transform.position, closestEnemy.transform.position) < triggerRange)
                        {
                            enemyState = EnemyState.Chase;
                            targetEnemy = closestEnemy;
                            return;
                        }
                    }

                    var randomDirection = UnityEngine.Random.Range(0, 2);
                    var randomRange = UnityEngine.Random.Range(1, 2);
                    navMeshAgentWrapper.SetDestination(transform.position +
                                                       new Vector3(randomRange * (randomDirection == 0 ? 1 : -1), 0,
                                                           randomRange * (randomDirection == 0 ? 1 : -1)));


                    break;
                case EnemyState.Chase:
                    navMeshAgentWrapper.SetDestination(targetEnemy.transform.position);
                    if (Vector3.Distance(transform.position, targetEnemy.transform.position) < attackRange)
                    {
                        enemyState = EnemyState.Attack;
                    }

                    if (Vector3.Distance(transform.position, targetEnemy.transform.position) > 4f &&
                        !navMeshAgentWrapper.Visible(targetEnemy.gameObject))
                    {
                        enemyState = EnemyState.Idle;
                    }

                    break;
                case EnemyState.Attack:
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        targetEnemy.Damage(); // todo: weapon decide
                        enemyState = EnemyState.Idle;
                    }

                    break;
            }
        }

        private EnemyHate GetClosestEnemy()
        {
            var closestEnemy = enemyHateList[0];
            foreach (EnemyHate enemyHate in enemyHateList)
            {
                if (Vector3.Distance(transform.position, enemyHate.transform.position) <
                    Vector3.Distance(transform.position, closestEnemy.transform.position))
                {
                    closestEnemy = enemyHate;
                }
            }

            return closestEnemy;
        }

        #region destroy

        UnityAction onDestroy;

        public void AddDestroyListener(UnityAction func)
        {
            onDestroy += func;
        }

        #endregion
    }
}