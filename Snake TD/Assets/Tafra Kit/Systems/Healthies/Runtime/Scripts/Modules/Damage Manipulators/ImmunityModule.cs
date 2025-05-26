using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Damage Manipulators/Immunity")]
    public class ImmunityModule : HealthyModule
    {
        [SerializeField] private bool immuneByDefault;

        [Header("VFX")]
        [SerializeField] private ParticleSystem immunePSPrefab;

        private ParticleSystem immunePS;
        private ControlReceiver immunityActivators;
        private bool isImmune;
        private HitInfo immuneHit = new HitInfo(0);

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;
        public override bool DisableOnDeath => true;

        protected override void OnInitialize()
        {
            immunityActivators = new ControlReceiver(OnFirstImmunityActivatorAdded, null, OnAllImmunityActivatorsCleared);

            if (healthy.Events.EnableAboutToTakeDamageEvent == false)
                TafraDebugger.Log("Immunity Module", "The healthy I'm attached to doesn't have \"OnAboutToTakeDamage\" event enabled. I won't work.", TafraDebugger.LogType.Error);

            if (immunePSPrefab != null)
            {
                immunePS = GameObject.Instantiate(immunePSPrefab, healthy.transform);
                immunePS.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
        protected override void OnEnable()
        {
            if(immuneByDefault)
                AddImmunityActivator("immuneByDefault");
            else if(immunityActivators.HasAnyController())
                EnableImmunity();
        }
        protected override void OnDisable()
        {
            immunityActivators.RemoveAllControllers();

            if (isImmune)
                DisableImmunity();
        }

        private void OnFirstImmunityActivatorAdded()
        {
            EnableImmunity();
        }
        private void OnAllImmunityActivatorsCleared()
        {
            DisableImmunity();
        }

        private void EnableImmunity()
        {
            if(isImmune)
                return;

            isImmune = true;

            if (immunePS != null)
                immunePS.Play();

            healthy.Events.OnAboutToTakeDamage.AddListener(OnAboutToTakeDamage);
        }
        private void DisableImmunity()
        {
            if(!isImmune)
                return;

            isImmune = false;

            if (immunePS != null)
                immunePS.Stop();

            healthy.Events.OnAboutToTakeDamage.RemoveListener(OnAboutToTakeDamage);
        }

        private void OnAboutToTakeDamage(Healthy healthy, HitEventArgs args)
        {
            if(isImmune)
                args.OverrideHitInfo(immuneHit);
        }

        public void AddImmunityActivator(string activatorID)
        {
            if (!isEnabled)
                return;

            immunityActivators.AddController(activatorID);
        }
        public void RemoveImmunityActivator(string activatorID)
        {
            if (!isEnabled)
                return;

            immunityActivators.RemoveController(activatorID);
        }
    }
}