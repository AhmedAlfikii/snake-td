using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [System.Serializable]
    public class AbilityUpgradeablePropertiesContainer
    {
        [SerializeReference] private List<AbilityUpgradeableProperty> upgradeableProperties = new List<AbilityUpgradeableProperty>();

        public List<AbilityUpgradeableProperty> UpgradeableProperties => upgradeableProperties;
    }
}