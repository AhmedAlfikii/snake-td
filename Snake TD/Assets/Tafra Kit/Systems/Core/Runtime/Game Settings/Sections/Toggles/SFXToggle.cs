using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    public class SFXToggle : GameSettingsToggle
    {
        protected override bool IsManagerOn => !SFXPlayer.IsMuted;

        public override bool AreConditionsSatisfied()
        {
            return SFXPlayer.IsEnabled;
        }

        protected override void OnValueChange(bool on)
        {
            SFXPlayer.IsMuted = !on;
        }
    }
}