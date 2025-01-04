using Enemy.enemyState;
using UnityEngine;

namespace Enemy
{
    public class Rabbit : Enemy
    {
        public ParticleSystem particleSystem;
        private void Start()
        {
            base.Start();
            particleSystem = GetComponentInChildren<ParticleSystem>();
            particleSystem.Stop();
        }

        protected override string GetSFXName()
        {
            return "openBag";
        }

        private void Update()
        {
            base.Update();
        }
        protected override void GoToAttack()
        {
            iEnemyState = new RabbitAttackState();
        }
    }
}