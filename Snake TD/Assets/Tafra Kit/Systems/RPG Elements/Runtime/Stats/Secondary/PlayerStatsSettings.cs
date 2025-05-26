using System.Collections.Generic;
using TafraKit.ContentManagement;
using UnityEngine;

namespace TafraKit.RPG
{
    public class PlayerStatsSettings : SettingsModule
    {
        [System.Serializable]
        public class StatSettings
        {
            public TafraAsset<Stat> stat;
            public IntRounding rounding;
        }

        [SerializeField] private List<StatSettings> usedStats;

        public List<StatSettings> UsedSats => usedStats;
        public override string Name => "RPG/Player Stats";
        public override string Description => "Define the stats that are used by the player so that other systems might reference them (doesn't actually affect the stats and whether they work or not).";
    }
}