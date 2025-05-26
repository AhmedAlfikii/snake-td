using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    [System.Serializable]
    public class RepeatingUpgradeModulesContainer
    {
        [SerializeReference] private List<RepeatingUpgradeModule> modules = new List<RepeatingUpgradeModule>();

        public List<RepeatingUpgradeModule> Modules => modules;
    }
}