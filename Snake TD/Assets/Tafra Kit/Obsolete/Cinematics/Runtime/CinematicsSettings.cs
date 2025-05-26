using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class CinematicsSettings : SettingsModule
    {
        public bool Enabled;
        public bool DisplayCinematicBars = true;
        public bool DisableInputsDuringCinematics = true;
        public override int Priority => 6;

        public override string Name => "Narration/Cinematics";

        public override string Description => "Control how cinematics will behave.";
    }
}