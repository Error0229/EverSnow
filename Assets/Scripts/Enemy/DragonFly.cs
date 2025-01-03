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
            if (particleSystem)
            {
                particleSystem = GetComponentInChildren<ParticleSystem>();
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
    }
}