using System;
using System.Diagnostics;
using UnityEditor;

namespace Enemy.enemyState
{
    public interface IEnemyState
    {
        public void OnEnter(Enemy enemy);
        public void OnExit(Enemy enemy);
        public void OnUpdate(Enemy enemy);
    }
}