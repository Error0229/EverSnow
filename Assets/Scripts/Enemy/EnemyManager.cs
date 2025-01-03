using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        List<BeeSpawnPoint> beeSpawnPoints = new List<BeeSpawnPoint>();
        private List<RabbitSpawnPoint> rabbitSpawnPoints = new();
        List<DragonFlySpawnPoint> dragonSpawnPoints = new();
        List<Enemy> enemies = new List<Enemy>();
        private Enemy beePrefab;
        private Enemy dragonPrefab;
        private Enemy rabbitPrefab;


        private void Start()
        {
            beeSpawnPoints.Clear();
            dragonSpawnPoints.Clear();
            rabbitSpawnPoints.Clear();
            foreach (var spawnPoint in FindObjectsOfType<DragonFlySpawnPoint>())
            {
                dragonSpawnPoints.Add(spawnPoint);
            }

            foreach (var spawnPoint in FindObjectsOfType<RabbitSpawnPoint>())
            {
                rabbitSpawnPoints.Add(spawnPoint);
            }

            foreach (var spawnPoint in FindObjectsOfType<BeeSpawnPoint>())
            {
                beeSpawnPoints.Add(spawnPoint);
            }

            beePrefab = Resources.Load<Enemy>("Bee");
            dragonPrefab = Resources.Load<Enemy>("DragonFly");
            rabbitPrefab = Resources.Load<Enemy>("Rabbit");
        }

        private void Update()
        {
            var limit = 20;
            if (enemies.Count < limit)
            {
                var random = UnityEngine.Random.Range(0, 3);
                if (random == 0)
                {
                    SpawnBee();
                }
                else if (random == 1)
                {
                    SpawnDragon();
                }
                else
                {
                    SpawnRabbit();
                }
            }
        }

        private void SpawnDragon()
        {
            var spawnPoint = dragonSpawnPoints[UnityEngine.Random.Range(0, dragonSpawnPoints.Count)];
            var enemy = Instantiate(dragonPrefab, spawnPoint.GetPosition(), Quaternion.identity);
            enemy.AddDestroyListener(() => enemies.Remove(enemy));
            enemies.Add(enemy);
        }

        private void SpawnRabbit()
        {
            var spawnPoint = rabbitSpawnPoints[UnityEngine.Random.Range(0, rabbitSpawnPoints.Count)];
            var enemy = Instantiate(rabbitPrefab, spawnPoint.GetPosition(), Quaternion.identity);
            enemy.AddDestroyListener(() => enemies.Remove(enemy));
            enemies.Add(enemy);
        }

        void SpawnBee()
        {
            var spawnPoint = beeSpawnPoints[UnityEngine.Random.Range(0, beeSpawnPoints.Count)];
            // var randomTwoOrThree = UnityEngine.Random.Range(2, 4);
            var randomTwoOrThree = 1;
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