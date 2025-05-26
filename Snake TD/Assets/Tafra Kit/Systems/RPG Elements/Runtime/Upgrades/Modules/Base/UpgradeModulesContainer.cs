using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    [System.Serializable]
    public class UpgradeModulesContainer
    {
        [SerializeReference] private List<UpgradeModule> modules = new List<UpgradeModule>();

        public List<UpgradeModule> Modules => modules;
    }
}