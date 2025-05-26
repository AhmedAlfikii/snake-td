using System.Collections;
using System.Collections.Generic;
using TafraKit.Consumables;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit.ExternalSDKs.BalancySDK
{
    [System.Serializable]
    public class BalancyItemStorable
    {
        [Tooltip("The ID of the item in Balancy")]
        public int unnyID;
        public StorableScriptableObject storable;
    }
}