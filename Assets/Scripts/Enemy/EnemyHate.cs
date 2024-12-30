using System;
using UnityEngine;

namespace Enemy
{
    public  class EnemyHate: MonoBehaviour
    {
        private bool isPlayer = false;
        Player player;
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
            player = GetComponent<Player>();
            if (player == null)
            {
                isPlayer = false;
            }
            else
            {
                isPlayer = true;
            }
        }

        // private int hp = 3;
        public Vector3 GetPosition()
        {
            return transform.position;

        }

        public void Damage()
        {
            if(isPlayer)
            {
                player.Health--;
            }
        }
    }
}