using System.Collections.Generic;
using TafraKit.CharacterControls;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Character Controls/Disable Controls On Death")]
    public class DisableControlsOnDeathModule : HealthyModule
    {
        private ICharacterController characterController;

        public override bool DisableOnDeath => false;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            characterController = healthy.GetComponent<ICharacterController>();
        }
        protected override void OnEnable()
        {
            healthy.Events.OnDeath.AddListener(OnDeath);
            healthy.Events.OnRevive.AddListener(OnRevive);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnDeath.RemoveListener(OnDeath);
            healthy.Events.OnRevive.RemoveListener(OnRevive);
        }

        private void OnDeath(Healthy healthy, HitInfo killHit)
        {
            characterController.ToggleAllControlCategories(false, "deathModule");
        }
        private void OnRevive()
        {
            characterController.ToggleAllControlCategories(true, "deathModule");
        }
    }
}