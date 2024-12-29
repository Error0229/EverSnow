using UnityEngine;

namespace Enemy
{
    public class SpawnPoint : MonoBehaviour
    {
        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}