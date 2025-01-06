using Enemy.enemyState;
using UnityEngine;

namespace Enemy
{
    public class DragonFly : Enemy
    {
        public ParticleSystem particleSystem;

        private void Start()
        {
            base.Start();
            particleSystem = GetComponentInChildren<ParticleSystem>();
            if (particleSystem)
            {
                particleSystem.Stop();
            }
        }

        private void Update()
        {
            base.Update();
        }

        public override float GetDefaultHeight()
        {
            return 1.5f;
        }

        protected override void GoToAttack()
        {
            iEnemyState = new RabbitAttackState();
        }
    }
}