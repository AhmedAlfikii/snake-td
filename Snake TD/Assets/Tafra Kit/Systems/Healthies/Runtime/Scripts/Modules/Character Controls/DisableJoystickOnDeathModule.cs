using System.Collections.Generic;
using TafraKit.CharacterControls;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Character Controls/Disable Joystick On Death")]
    public class DisableJoystickOnDeathModule : HealthyModule
    {
        private OnScreenJoystick joystick;

        public override bool DisableOnDeath => false;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
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
            if (joystick == null)
            {
                joystick = SceneReferences.Joystick;

                if(joystick == null)
                    return;
            }

            joystick.Deactivate();
        }
        private void OnRevive()
        {
            if(joystick == null)
            {
                joystick = SceneReferences.Joystick;

                if(joystick == null)
                    return;
            }

            joystick.Activate();
        }
    }
}