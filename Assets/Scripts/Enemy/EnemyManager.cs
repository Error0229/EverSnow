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

            foreach (var bsp in beeSpawnPoints) { SpawnBee(bsp); }
            foreach (var rsp in rabbitSpawnPoints) { SpawnRabbit(rsp); }
            foreach (var dsp in dragonSpawnPoints) { SpawnDragon(dsp); }
            
        }

        private void Update()
        {
            var limit = 20 + dragonSpawnPoints.Count + rabbitSpawnPoints.Count + beeSpawnPoints.Count;
            if (enemies.Count < limit)
            {
                var random = UnityEngine.Random.Range(0, 3);
                if (random == 0)
                {
                    RandomSpawnBee();
                }
                else if (random == 1)
                {
                    RandomSpawnDragon();
                }
                else
                {
                    RandomSpawnRabbit();
                }
            }
        }

        private void RandomSpawnDragon()
        {
            if (dragonSpawnPoints.Count == 0)
                return;
            var spawnPoint = dragonSpawnPoints[UnityEngine.Random.Range(0, dragonSpawnPoints.Count)];
            SpawnDragon(spawnPoint);
        }
        
        void SpawnDragon(DragonFlySpawnPoint spawnPoint)
        {
            var enemy = Instantiate(dragonPrefab, spawnPoint.GetPosition(), Quaternion.identity);
            enemy.AddDestroyListener(() => enemies.Remove(enemy));
            enemies.Add(enemy);
        }

        private void RandomSpawnRabbit()
        {
            if (rabbitSpawnPoints.Count == 0)
                return;
            var spawnPoint = rabbitSpawnPoints[UnityEngine.Random.Range(0, rabbitSpawnPoints.Count)];
            SpawnRabbit(spawnPoint);
        }

        private void SpawnRabbit(SpawnPoint spawnPoint)
        {
            var enemy = Instantiate(rabbitPrefab, spawnPoint.GetPosition(), Quaternion.identity);
            enemy.AddDestroyListener(() => enemies.Remove(enemy));
            enemies.Add(enemy);
        }
        
        

        void RandomSpawnBee()
        {
            if (beeSpawnPoints.Count == 0)
                return;
            var spawnPoint = beeSpawnPoints[UnityEngine.Random.Range(0, beeSpawnPoints.Count)];
            SpawnBee(spawnPoint);
        }

        void SpawnBee(SpawnPoint spawnPoint)
        {
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