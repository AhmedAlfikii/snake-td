using System.Collections.Generic;
using UnityEngine;
using TafraKit.Roguelike;

namespace TafraKit.Internal.Roguelike
{
    public class PerksHandlerSettings : SettingsModule
    {
        [SerializeField] private bool enabled;
        [SerializeField] private List<PerksGroup> perkGroups;

        public bool Enabled => enabled;
        public List<PerksGroup> PerkGroups => perkGroups;

        public override string Name => "Roguelike/Perks Handler";
        public override string Description => "Control perks";
    }
}