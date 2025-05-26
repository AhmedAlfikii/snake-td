using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.SceneManagement
{
    public class SceneManagerSettings : SettingsModule
    {
        public GameObject DefaultTransition;

        public override int Priority => 10;

        public override string Name => "General/Scene Manager";

        public override string Description => "Control transitions between scene switches.";
    }
}