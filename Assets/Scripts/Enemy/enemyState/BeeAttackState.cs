using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Wrapper;

namespace Enemy.enemyState
{
    public class BeeAttackState : AttackState
    {
        ParticleSystem parti;

        public override void OnEnter(Enemy enemy)
        {
            parti = enemy.GetParticle();
            if (parti)
            {
                parti.Play();
            }

            base.OnEnter(enemy);
        }

        public override void OnExit(Enemy enemy)
        {
            if (parti)
            {
                parti.Stop();
            }

            base.OnExit(enemy);
        }

        public override void OnUpdate(Enemy enemy)
        {
            base.OnUpdate(enemy);
        }
    }
}