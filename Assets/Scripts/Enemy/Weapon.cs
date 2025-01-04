using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Weapon : MonoBehaviour
    {
        List<EnemyHate> hitedEnemies = new List<EnemyHate>();
        public bool IsHited => hitedEnemies.Count > 0;

        private void Start()
        {
            hitedEnemies.Clear();
        }

        public void Reset()
        {
            hitedEnemies.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponent<EnemyHate>();
            if (target)
            {
                if (hitedEnemies.Contains(target))
                    return;
                var con = target.GetComponentInChildren<BetterPlayerController>();
                if (con)
                {
                    con.Knockback(transform.position);
                }
                target.Damage();
                hitedEnemies.Add(target);
            }
        }
    }
}