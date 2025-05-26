using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Consumables
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "Tafra Kit/Consumables/Consumables Reward")]
    public class ConsumablesReward : ScriptableObject
    {
        [SerializeField] private List<ConsumableChange> rewards;

        public List<ConsumableChange> Rewards => rewards;
    }
}