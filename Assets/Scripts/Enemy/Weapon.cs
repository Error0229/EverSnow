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
            print(other.name);
            if (target)
            {
                if (hitedEnemies.Contains(target))
                    return;
                target.Damage();
                hitedEnemies.Add(target);
            }
        }
    }
}