using System.Collections.Generic;
using TafraKit.ContentManagement;
using UnityEngine;

namespace TafraKit.Internal
{
    public class GlobalManagersHandlerSettings : SettingsModule
    {
        [SerializeField] private List<TafraAsset<GlobalManager>> managers;

        public List<TafraAsset<GlobalManager>> Managers => managers;

        public override string Name => "General/Global Managers";
        public override string Description => "Control the game objects that should be spawned in every scene.";
        public override int Priority => 0;
    }
}