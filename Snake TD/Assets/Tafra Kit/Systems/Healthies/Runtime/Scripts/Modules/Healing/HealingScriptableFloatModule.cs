using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ContentManagement;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Healing/Healing Scriptable Float")]
    public class HealingScriptableFloatModule : HealthyModule
    {
        [SerializeField] private TafraAsset<ScriptableFloat> healingFloatAsset;
        [Tooltip("If true, the value inside the Scriptable Float will be treated as a percentage, so if 0.5 are added, then the healthy will be healed by 50%.")]
        [SerializeField] private bool floatIsPercentage = true;

        [NonSerialized] private ScriptableFloat healingFloat;

        public override bool DisableOnDeath => true;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            healingFloat = healingFloatAsset.Load();

            healingFloat.OnValueAdd.AddListener(OnValueAdded);
        }
        protected override void OnDisable()
        {
            healingFloat.OnValueAdd.RemoveListener(OnValueAdded);

            healingFloatAsset.Release();
        }

        private void OnValueAdded(float addedValue)
        {
            if (floatIsPercentage)
                healthy.Heal(Mathf.CeilToInt(addedValue * healthy.CurrentMaxHealth));
            else
                healthy.Heal(addedValue);
        }
    }
}