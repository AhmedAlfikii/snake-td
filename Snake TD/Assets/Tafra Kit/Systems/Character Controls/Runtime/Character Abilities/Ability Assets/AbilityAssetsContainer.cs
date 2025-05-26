using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [System.Serializable]
    public class AbilityAssetsContainer
    {
        [SerializeReference] private List<AbilityAssetModule> modules = new List<AbilityAssetModule>();

        public List<AbilityAssetModule> Modules => modules;
    }
}