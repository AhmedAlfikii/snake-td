using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Vibration;

namespace TafraKit.Internal
{
    public class VibrationToggle : GameSettingsToggle
    {
        #if TAFRA_VIBRATION
        protected override bool IsManagerOn => !TafraVibration.IsMuted;
        #else
        protected override bool IsManagerOn => false;
        #endif

        public override bool AreConditionsSatisfied()
        {
            #if TAFRA_VIBRATION
            return TafraVibration.IsEnabled;
            #else
            return false;
            #endif
        }

        protected override void OnValueChange(bool on)
        {
            #if TAFRA_VIBRATION
            TafraVibration.IsMuted = !on;
            #endif
        }
    }
}