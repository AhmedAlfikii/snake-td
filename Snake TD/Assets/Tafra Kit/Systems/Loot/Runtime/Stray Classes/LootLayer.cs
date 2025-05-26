using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Loot
{
    [Serializable]
    public class LootLayer
    {
        [Range(0, 1)] 
        [SerializeField] private float probability;
        [SerializeField] private LootItem[] items;

        public float Probability => probability;
        public LootItem[] Items => items;
    }
}