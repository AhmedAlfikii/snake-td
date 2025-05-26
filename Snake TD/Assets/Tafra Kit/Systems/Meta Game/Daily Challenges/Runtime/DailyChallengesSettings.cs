using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Consumables;

namespace TafraKit.MetaGame
{
    public class DailyChallengesSettings : SettingsModule
    {
        public bool Enabled = false;
        [Tooltip("The index of the start and end scenes of the challenges in the scenes in build. A random scene will be selected from them whenever a challenge should start.")]
        public IntRange ChallengeSceneIndicesRange;
        [Tooltip("The index of the scene that should be opened when the scene is closing.")]
        public int ExistSceneIndex = 1;

        [Header("Month Rewards")]
        public int[] MonthRewardDays = new int[] { };
        public ConsumableChangeGroup[] MonthRewards = new ConsumableChangeGroup[] { };

        public override int Priority => 21;

        public override string Name => "Meta Game/Daily Challenges";

        public override string Description => "Daily Challenges settings";
    }
}