using UnityEngine;

namespace TafraKit.Internal
{
    public class SceneReferencesSettings : SettingsModule
    {
        [SerializeField] private bool enabled = true;

        public bool Enabled => enabled;

        public override string Name => "Utilities/Scene References";
        public override string Description => "Improves the performance of accessing main sacene objects.";

    }
}