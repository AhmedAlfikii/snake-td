using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Healthy/Revive")]
    public class HealthyReviveTestModule : ActionOnInputTestingModule
    {
        private Healthy healthy;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            healthy = characterTesting.GetComponent<Healthy>();
        }

        protected override void OnInputReceived()
        {
            healthy.Revive();
        }
    }
}