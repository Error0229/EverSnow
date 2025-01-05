using System;
using System.Collections.Generic;
using Enemy.enemyState;
using Unity.VisualScripting;
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

        [SerializeField, SerializeReference] private IEnemyState _iEnemyState;
        float sfxTime = 0;
        float sfxMaxTime = 2f;

        public void SetSfxMaxTime(float v)
        {
            sfxMaxTime = v;
        }
        public float GetSfxMaxTime() => sfxMaxTime;

        public IEnemyState iEnemyState
        {
            get => _iEnemyState;
            set
            {
                _iEnemyState?.OnExit(this);
                _iEnemyState = value;
                _iEnemyState?.OnEnter(this);
            }
        }

        virtual protected string GetSFXName()
        {
            return "FootStep";
        }
        public void PlaySFX()
        {
            var soundManager = AudioManager.Instance;
            soundManager.PlaySFX(GetSFXName(), transform.position, 0.5f);
        }

        private List<EnemyHate> enemyHateList = new();
        public NavMeshAgentWrapper navMeshAgentWrapper;

        public EnemyHate targetEnemy;
        public Vector3 targetPosition;

        // private NavMeshAgent navMeshAgent;
        public Animator animator;
        public Collider weaponCollider;
        public Weapon weapon;

        public Collider selfCollider;
        private int health;
        private int maxHealth;

        protected void Start()
        {
            view = transform.GetChild(0).gameObject;
            animator = GetComponentInChildren<Animator>();
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgentWrapper = new NavMeshAgentWrapper(navMeshAgent);

            EnemyHate[] enemyHate = GameObject.FindObjectsByType<EnemyHate>(FindObjectsSortMode.None);
            enemyHateList = new List<EnemyHate>(enemyHate);

            weapon = GetComponentInChildren<Weapon>();
            weaponCollider = weapon.gameObject.GetComponent<Collider>();
            if (weaponCollider == null)
            {
                throw new Exception("Weapon collider not found");
            }
            selfCollider = gameObject.GetComponent<Collider>();
            if (selfCollider == null)
            {
                throw new Exception("The enemy collider is not found");
            }
            maxHealth = 1;
            health = 1;
            iEnemyState = new IdleState();
        }

        private float runtime = 0;
        private float maxRuntime = 3;

        public virtual float GetDefaultHeight()
        {
            return 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Harmful"))
            {
                health--;
            }
            if (health <= 0)
            {
                Die();
            }
        }
        private void Die()
        {
            Deadge.SpawnDeadgeEffect(transform.position);
            Destroy(this.gameObject);
        }

        protected void Update()
        {
            iEnemyState?.OnUpdate(this);
            if (sfxTime < GetSfxMaxTime())
                sfxTime += Time.deltaTime;
            else
            {
                sfxTime = 0;
                PlaySFX();
            }
        }

        public float GetAttackRange()
        {
            var attackRange = 8f;
            return attackRange;
        }

        #region destroy

        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }

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
                    GoToIdle();
                    break;
                case EnemyState.Chase:
                    iEnemyState = new ChaseState();
                    break;
                case EnemyState.Attack:
                    GoToAttack();
                    break;
                case EnemyState.Away:
                    iEnemyState = new AwayState();
                    break;
            }
        }

        protected virtual void GoToAttack()
        {
            iEnemyState = new AttackState();
        }
        protected virtual void GoToIdle()
        {
            iEnemyState = new IdleState();
        }

        public virtual ParticleSystem GetParticle()
        {
            return null;
        }

        public void ResetSfxMaxTime()
        {
            sfxMaxTime = 5f;
        }
    }
}
