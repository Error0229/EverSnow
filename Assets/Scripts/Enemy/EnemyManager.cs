using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
        List<Enemy> enemies = new List<Enemy>();
        private Enemy beePrefab;


        private void Start()
        {
            spawnPoints.Clear();
            foreach (var spawnPoint in FindObjectsOfType<SpawnPoint>())
            {
                spawnPoints.Add(spawnPoint);
            }

            beePrefab = Resources.Load<Enemy>("Bee");
        }

        private void Update()
        {
            if (enemies.Count < 3)
            {
                var spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
                var randomTwoOrThree = UnityEngine.Random.Range(2, 4);
                var spawnRange = UnityEngine.Random.Range(1, 2);
                var randomDirection = UnityEngine.Random.Range(0, 2);
                for (int i = 0; i < randomTwoOrThree; i++)
                {
                    var enemy = Instantiate(beePrefab,
                        spawnPoint.GetPosition() + new Vector3(spawnRange * (randomDirection == 0 ? 1 : -1), 0,
                            spawnRange * (randomDirection == 0 ? 1 : -1))
                        , Quaternion.identity);
                    enemy.AddDestroyListener(() => enemies.Remove(enemy));
                    enemies.Add(enemy);
                }
            }
        }
    }
}