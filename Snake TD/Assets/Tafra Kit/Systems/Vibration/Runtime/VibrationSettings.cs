using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Vibration
{
    public class VibrationSettings : SettingsModule
    {
        public bool Enabled = true;
        public bool MutedByDefault;

        public override int Priority => 9;

        public override string Name => "Mobile/Vibration";

        public override string Description => "Control how vibrations are made in the game.";
    }
}
