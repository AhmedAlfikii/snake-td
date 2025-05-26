using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZUI;
using TafraKit.Consumables;

namespace TafraKit
{
    public class ConsumablesSettings : SettingsModule
    {
        [System.Serializable]
        public class ConsumableBarByConsumable
        {
            public Consumable consumable;
            public ConsumableUIBar barPrefab;
        }

        public OutOfConsumablePopup OutOfConsumablesPopup;
        public Consumable[] mainHardCurrencies;
        public ConsumableBarByConsumable[] consumableBarByConsumable;

        public override int Priority => 20;

        public override string Name => "Resource Management/Consumables";

        public override string Description => "Economy handling.";
    }
}
