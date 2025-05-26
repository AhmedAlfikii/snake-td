using System;
using System.Collections.Generic;
using TafraKit.Consumables;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit
{
    public class ItemRewarderSettings : SettingsModule
    {
        [Serializable]
        public class ExpandableStorable
        {
            public StorableScriptableObject storable;
            public List<StorableScriptableObject> expantionList;
        }
        [Serializable]
        public class IdleIncomeConsumable
        {
            public Consumable idleIncomeToken;
            public Consumable mainConsumable;
        }

        public bool Enabled = false;

        [Header("UI")] 
        public bool FreezeTimeScale = true;
        public RewardsScreen ScreenPrefab;
        public bool PlayConsumablesAnimation = true;
        
        [Header("SFX")]
        public SFXClip ShowScreenSFX;
        public SFXClip ClaimSFX;

        [Header("Expandable Items")]
        public List<ExpandableStorable> ExpandableStorables = new List<ExpandableStorable>();

        [Header("Idle Income Items")]
        public List<IdleIncomeConsumable> IdleIncomeConsumables = new List<IdleIncomeConsumable>();

        public override int Priority => 11;
        public override string Name => "Resource Management/Item Rewarder";
        public override string Description => "Control how the rewards screen is displayed.";
    }
}
