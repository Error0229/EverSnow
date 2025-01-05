using System;
using UnityEngine;

namespace Enemy
{
    public class Deadge : MonoBehaviour
    {
        static public void SpawnDeadgeEffect(Vector3 position)
        {
            var deadge = Resources.Load<GameObject>("Deadge");
            Instantiate(deadge, position, Quaternion.identity);
        }
        ParticleSystem particleSystem;
        private void Start()
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();
            particleSystem.Play();
        }

        private void Update()
        {
            if (!particleSystem.isPlaying)
                Destroy(gameObject);
        }
    }
}