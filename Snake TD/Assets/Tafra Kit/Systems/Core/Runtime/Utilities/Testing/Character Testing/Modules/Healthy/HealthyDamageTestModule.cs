using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Healthy/Damage")]
    public class HealthyDamageTestModule : ActionOnInputTestingModule
    {
        [SerializeField] private int damage = 10;

        private Healthy healthy;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            healthy = characterTesting.GetComponent<Healthy>();
        }

        protected override void OnInputReceived()
        {
            healthy.TakeDamage(new HitInfo(damage));
        }
    }
}