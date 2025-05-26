#if TAFRA_IAP
using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Consumables;
using UnityEngine;
using UnityEngine.Purchasing;

namespace TafraKit.IAP
{
    [Serializable]
    public class IAPProductGameplayIData
    {
        public ConsumableChange Consumable;
        public int FirstTimePurchaseBonusAmount;
        public string ConsumableType = "hard";

        [NonSerialized] public string LastPlacement;
    }
}
#endif