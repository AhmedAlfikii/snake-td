using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Consumables
{
    [Serializable]
    public class WeightedConsumableRewardsGroup
    {
        public WeightedConsumableReward[] rewards;

        public int GetRandomRewardIndex()
        {
            //TODO: Actually return a weighted random reward.
            int rewardIndex = UnityEngine.Random.Range(0, rewards.Length);

            return rewardIndex;
        }
        public WeightedConsumableReward GetRandomReward()
        {
            return rewards[GetRandomRewardIndex()];
        }
    }
}