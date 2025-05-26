using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Loot
{
    [Serializable]
    public class LootItem
    {
        public LootData lootData;
        public IntRange amount;

        public int GetValue()
        {
            return amount.GetRandomValue();
        }
    }
}