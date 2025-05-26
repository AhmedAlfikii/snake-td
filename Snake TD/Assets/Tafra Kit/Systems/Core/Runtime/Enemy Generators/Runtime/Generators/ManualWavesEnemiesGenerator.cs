using System;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;

namespace TafraKit.Internal
{
    [CreateAssetMenu(menuName = "Tafra Kit/Enemies/Generators/Manual Waves", fileName = "Manual Waves Enemies Generator")]
    public class ManualWavesEnemiesGenerator : EnemiesGenerator
    {
        [Serializable]
        public class EnemyData
        {
            [Tooltip("What slot in the assigned enemy set should we get this enemy from?")]
            [SerializeField] private TafraString enemySlot;
            [Tooltip("What is the maximum number of enemies of this type that should be alive at the same time?")]
            [SerializeField] private int maxLiveCount;
            [Tooltip("How long should it take for this enemy to reach its maximum number of live units?")]
            [SerializeField] private float durationToReachMaxCount;

            public string EnemySlot => enemySlot.Value;
            public int MaxLiveCount => maxLiveCount;
            public float DurationToReachMaxCount => durationToReachMaxCount;
        }

        [Serializable]
        public class Wave
        {
            [Tooltip("The total duration of the waves in seconds.")]
            [SerializeField] private int duration = 45;
            [SerializeField] private List<EnemyData> enemies;

            public int Duration => duration;
            public List<EnemyData> Enemies => enemies;
        }

        [SerializeField] private List<Wave> waves = new List<Wave>();

        public List<Wave> Waves => waves;
    }
}