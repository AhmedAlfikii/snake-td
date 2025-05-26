using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Loot
{
    public class HealthyLootSource : LootSource
    {
        private Healthy healthy;

        protected override void Awake()
        {
            base.Awake();

            healthy = GetComponent<Healthy>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            healthy.Events.OnDeath.AddListener(OnHealthyDeath);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            healthy.Events.OnDeath.RemoveListener(OnHealthyDeath);
        }

        private void OnHealthyDeath(Healthy healthy, HitInfo killerHit)
        {
            DropLoot();
        }
    }
}