using UnityEngine;

namespace Enemy
{
    public  class EnemyHate: MonoBehaviour
    {
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