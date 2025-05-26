using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class ProjectSetupSettings : SettingsModule
    {
        public override int Priority => 0;

        public override string Name => "Project Setup";

        public override string Description => "Setting up the essential parameters of the project's settings.";
    }
}