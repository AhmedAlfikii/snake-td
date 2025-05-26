using System;
using UnityEngine;
using TafraKit;

namespace TafraKit.Loot
{
    [CreateAssetMenu(menuName = "Tafra Kit/Loot/Loot Layers Data", fileName = "Loot Layers Data")]
    public class LootLayersData : ScriptableObject
    {
        #region Private Serialized Fields
        [SerializeField] private LootLayer[] layers;
        #endregion

        #region Public Properties
        public LootLayer[] Layers => layers;
        #endregion

        #region Public Methods
        public LootLayer GetLayer(int index)
        {
            if (index < 0 || index > layers.Length)
                return null;

            return layers[index];
        }
        #endregion
    }
}