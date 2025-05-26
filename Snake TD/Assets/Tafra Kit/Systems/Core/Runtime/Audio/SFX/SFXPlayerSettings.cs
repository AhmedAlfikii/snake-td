using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class SFXPlayerSettings : SettingsModule
    {
        public bool Enabled = true;
        [Tooltip("The default scale of the volume of all the SFXs (e.g. if this is set to 0.5, then any SFX that is set to play at 1 volume will actually play at 0.5, and if it's set to play at 0.5, it will play at 0.25, and so on).")]
        [Range(0, 1)]
        public float VolumeScale = 1;
        public bool MutedByDefault = false;

        public override int Priority => 4;

        public override string Name => "Audio/SFX Player";

        public override string Description => "Play sound effects and control their volume and mute state.";
    }
}
