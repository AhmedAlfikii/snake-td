using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Healthy/Heal")]
    public class HealthyHealTestModule : ActionOnInputTestingModule
    {
        [SerializeField] private int heal = 10;

        private Healthy healthy;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            healthy = characterTesting.GetComponent<Healthy>();
        }

        protected override void OnInputReceived()
        {
            healthy.Heal(heal);
        }
    }
}