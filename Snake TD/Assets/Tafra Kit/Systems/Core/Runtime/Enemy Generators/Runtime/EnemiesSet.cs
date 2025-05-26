using System;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.ContentManagement;
using UnityEngine;

namespace TafraKit.Internal
{
    [CreateAssetMenu(menuName = "Tafra Kit/Enemies/Generators/Enemies Set", fileName = "Enemies Set")]
    public class EnemiesSet : ScriptableObject
    {
        [Serializable]
        public class EnemyData
        {
            public TafraString slot;
            public TafraAsset<Enemy> enemy;
        }
        [Serializable]
        public class LoadedEnemyData
        {
            private Enemy enemy;
            private string slot;

            public Enemy Enemy => enemy;
            public string Slot => slot;

            public LoadedEnemyData(Enemy enemy, string slot)
            {
                this.enemy = enemy;
                this.slot = slot;
            }

            public void Clear()
            {
                enemy = null;
            }
        }

        [SerializeField] private List<EnemyData> enemiesData = new List<EnemyData>();

        [NonSerialized] private List<LoadedEnemyData> loadedEnemies = new List<LoadedEnemyData>();
        [NonSerialized] private bool isLoaded;
        [NonSerialized] private int loaders;
        [NonSerialized] private Dictionary<string, Enemy> loadedEnemiesBySlot = new Dictionary<string, Enemy>();

        public List<LoadedEnemyData> EnemiesData
        {
            get
            {
                if(!isLoaded)
                {
                    TafraDebugger.Log("Enemies Set", "Enemies are not loaded. Call Load() first. And make sure to call Release() when you no longer need the enemies.", TafraDebugger.LogType.Error, this);
                    return null;
                }

                return loadedEnemies;
            }
        }

        public void Load()
        {
            loaders++;

            if(isLoaded)
                return;

            for (int i = 0; i < enemiesData.Count; i++)
            {
                var enemyData = enemiesData[i];
                Enemy enemy = enemyData.enemy.Load();
                string slot = enemyData.slot.Value;
                loadedEnemies.Add(new LoadedEnemyData(enemy, slot));
                loadedEnemiesBySlot.Add(slot, enemy);
            }

            isLoaded = true;
        }
        public void Release()
        {
            if(!isLoaded)
                return;
            
            loaders--;

            if(loaders <= 0)
            {
                loaders = 0;

                for (int i = 0; i < loadedEnemies.Count; i++)
                {
                    loadedEnemies[i].Clear();
                }

                loadedEnemies.Clear();
                loadedEnemiesBySlot.Clear();

                for (int i = 0; i < enemiesData.Count; i++)
                {
                    enemiesData[i].enemy.Release();
                }
            }

            isLoaded = false;
        }

        public bool TryGetEnemyBySlot(string slot, out Enemy enemy)
        {
            return loadedEnemiesBySlot.TryGetValue(slot, out enemy);
        }
    }
}