using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    public class MusicToggle : GameSettingsToggle
    {
        protected override bool IsManagerOn => !MusicPlayer.IsMuted;

        public override bool AreConditionsSatisfied()
        {
            return MusicPlayer.IsEnabled;
        }

        protected override void OnValueChange(bool on)
        {
            MusicPlayer.IsMuted = !on;
        }
    }
}