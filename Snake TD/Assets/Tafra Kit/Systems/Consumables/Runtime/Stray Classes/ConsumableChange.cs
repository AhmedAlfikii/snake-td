using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Consumables
{
    [Serializable]
    public class ConsumableChange
    {
        public Consumable consumable;
        public float changeAmount;

        public ConsumableChange() { }
        public ConsumableChange(Consumable consumable, float changeAmount)
        {
            this.consumable = consumable;
            this.changeAmount = changeAmount;
        }

        public void AddChange()
        {
            consumable.Add(changeAmount);
        }
        public void DeductChange()
        {
            consumable.Deduct(changeAmount);
        }
        public bool IsAffordable()
        {
            bool affordable = consumable.Value >= Mathf.Abs(changeAmount);

            return affordable;
        }
    }
}