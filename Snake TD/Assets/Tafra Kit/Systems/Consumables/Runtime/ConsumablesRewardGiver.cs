using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Consumables
{
    public class ConsumablesRewardGiver : MonoBehaviour
    {
        [SerializeField] private ConsumablesReward rewards;

        public void GiveRewards()
        {
            Debug.LogError("You should give the player the reward here");
            //ConsumableRewarder.Instance.AddRewards(rewards.Rewards);
            //ConsumableRewarder.Instance.ShowScreen();
        }
    }
}