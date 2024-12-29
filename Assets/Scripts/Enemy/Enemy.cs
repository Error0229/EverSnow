using System;
using System.Collections.Generic;
using Enemy.enemyState;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Wrapper;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        public enum EnemyState
        {
            Idle,
            Chase,
            Attack,
            Away
        }

        private GameObject view;

        private IEnemyState _iEnemyState;

        private IEnemyState iEnemyState
        {
            get => _iEnemyState;
            set
            {
                _iEnemyState?.OnExit(this);
                _iEnemyState = value;
                _iEnemyState?.OnEnter(this);
            }
        }


        private List<EnemyHate> enemyHateList = new();
        public NavMeshAgentWrapper navMeshAgentWrapper;

        public EnemyHate targetEnemy;

        // private NavMeshAgent navMeshAgent;
        public Animator animator;
        public Collider weaponCollider;
        public Weapon weapon;

        protected void Start()
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

            iEnemyState = new IdleState();
        }

        private float runtime = 0;
        private float maxRuntime = 3;


        protected void Update()
        {
            iEnemyState.OnUpdate(this);
        }

        public float GetAttackRange()
        {
            var attackRange = 1f;
            return attackRange;
        }

        #region destroy

        UnityAction onDestroy;

        public void AddDestroyListener(UnityAction func)
        {
            onDestroy += func;
        }

        #endregion

        public void SetWeaponColliderEnable(bool b)
        {
            weaponCollider.enabled = b;
        }

        public GameObject GetView()
        {
            return view;
        }

        public List<EnemyHate> GetHateList()
        {
            return enemyHateList;
        }

        public bool IsArrived()
        {
            return navMeshAgentWrapper.IsArrived();
        }

        public void SetDestination(Vector3 transformPosition)
        {
            navMeshAgentWrapper.SetDestination(transformPosition);
        }

        public void SetTargetEnemy(EnemyHate closestEnemy)
        {
            targetEnemy = closestEnemy;
        }

        public void SetState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    iEnemyState = new IdleState();
                    break;
                case EnemyState.Chase:
                    iEnemyState = new ChaseState();
                    break;
                case EnemyState.Attack:
                    iEnemyState = new AttackState();
                    break;
                case EnemyState.Away:
                    iEnemyState = new AwayState();
                    break;
            }
        }
    }



}