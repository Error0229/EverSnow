using System;
using UnityEngine;

namespace Enemy
{
    public  class EnemyHate: MonoBehaviour
    {
        private void Awake()
        {
            if (!GetComponentInChildren<Collider>())
            {
                throw new Exception("EnemyHate must have a collider");
            }
            if (!GetComponentInChildren<Rigidbody>())
            {
                throw new Exception("EnemyHate must have a rigidbody");
            }
            

        }

        private int hp = 3;
        public Vector3 GetPosition()
        {
            return transform.position;

        }

        public void Damage()
        {
            hp--;
            if (hp <= 0)
            {
                // Destroy(gameObject);
                // gameOver()
            }
        }
    }
}