using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Weapon : MonoBehaviour
    {
        List<EnemyHate> hitedEnemies = new List<EnemyHate>();

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
                print(target);
                if (hitedEnemies.Contains(target))
                    return;
                target.Damage();
                print("Damage");
                hitedEnemies.Add(target);
            }
        }
    }
}