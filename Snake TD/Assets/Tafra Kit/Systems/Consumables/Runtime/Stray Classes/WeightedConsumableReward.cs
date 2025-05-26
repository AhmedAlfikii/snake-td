using System;

namespace TafraKit.Consumables
{
    [Serializable]
    public class WeightedConsumableReward
    {
        public Consumable consumable;
        public float percentage = 1;
        public float weight;
        
        public float GetAmount(float fullAmount)
        { 
            return percentage * fullAmount;
        }
    }
}