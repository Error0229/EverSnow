using System;
using System.Collections.Generic;
using Enemy.enemyState;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Wrapper;

namespace Enemy
{
    public class Bee : Enemy
    {
        public ParticleSystem particleSystem;

        public override ParticleSystem GetParticle()
        {
            return particleSystem;
        }
        private void Start()
        {
            base.Start();
            particleSystem = GetComponentInChildren<ParticleSystem>();
            particleSystem.Stop();
        }

        private void Update()
        {
            base.Update();
        }
        protected override void GoToAttack()
        {
            iEnemyState = new BeeAttackState();
        }
    }
}