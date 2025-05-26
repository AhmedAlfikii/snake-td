using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ContentManagement;
using UnityEngine.Events;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Damage Manipulators/Dodge")]
    public class DodgeModule : HealthyModule
    {
        [SerializeField] private TafraFloat dodgeChanceAsset;

        private UnityEvent<HitInfo> onDodge = new UnityEvent<HitInfo>();

        public UnityEvent<HitInfo> OnDodge => onDodge;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;
        public override bool DisableOnDeath => true;

        protected override void OnInitialize()
        {
            if (healthy.Events.EnableAboutToTakeDamageEvent == false)
                TafraDebugger.Log("Dodge Module", "The healthy I'm attached to doesn't have \"OnAboutToTakeDamage\" event enabled. I won't work.", TafraDebugger.LogType.Error);
        }
        protected override void OnEnable()
        {
            dodgeChanceAsset.LoadAsset();

            healthy.Events.OnAboutToTakeDamage.AddListener(OnAboutToTakeDamage);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnAboutToTakeDamage.RemoveListener(OnAboutToTakeDamage);

            dodgeChanceAsset.UnloadAsset();
        }

        private void OnAboutToTakeDamage(Healthy healthy, HitEventArgs args)
        {
            float chance = dodgeChanceAsset.Value;

            if (chance >= 1 || UnityEngine.Random.value < chance)
            {
                HitInfo newHitInfo = args.ManipulatedHitInfo;
                newHitInfo.isMissed = true;

                args.OverrideHitInfo(newHitInfo);

                onDodge?.Invoke(newHitInfo);
            }
        }
    }
}