using System.Collections;
using System.Collections.Generic;
using TafraKit.Consumables;
using UnityEngine;

namespace TafraKit.ExternalSDKs.BalancySDK
{
    [System.Serializable]
    public class BalancyItemConsumable
    {
        [Tooltip("The ID of the item in Balancy")]
        public int unnyID;
        public Consumable consumable;
    }
}