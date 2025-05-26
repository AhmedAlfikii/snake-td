using System;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Healthy/Immunity")]
    public class HealthyImmunityTestModule : ActionOnInputTestingModule
    {
        [SerializeField] private bool immuneByDefault;

        private Healthy healthy;
        private bool isImmune;
        private bool isListeningToHealthy;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            healthy = characterTesting.GetComponent<Healthy>();

            if(immuneByDefault)
            {
                if(healthy.IsInitialized)
                    SetImmunity(true);
                else
                {
                    healthy.Events.OnInitialize.AddListener(OnHealthyInitialize);
                    isListeningToHealthy = true;
                }
            }
        }
        public override void OnDestroy()
        {
            base.OnDestroy();

            if(isListeningToHealthy)
            {
                healthy.Events.OnInitialize.RemoveListener(OnHealthyInitialize);
                isListeningToHealthy = false;
            }
        }

        private void OnHealthyInitialize()
        {
            if(immuneByDefault)
            {
                SetImmunity(true);
            }

            healthy.Events.OnInitialize.RemoveListener(OnHealthyInitialize);
            isListeningToHealthy = false;
        }

        protected override void OnInputReceived()
        {
            if(!isImmune)
                SetImmunity(true);
            else
                SetImmunity(false);
        }

        private void SetImmunity(bool on)
        {
            if(on && !isImmune)
            {
                healthy.GetModule<ImmunityModule>().AddImmunityActivator("healthy_immunity_test_module");
                isImmune = true;
            }
            else if (!on && isImmune)
            {
                healthy.GetModule<ImmunityModule>().RemoveImmunityActivator("healthy_immunity_test_module");
                isImmune = false;
            }
        }
    }
}