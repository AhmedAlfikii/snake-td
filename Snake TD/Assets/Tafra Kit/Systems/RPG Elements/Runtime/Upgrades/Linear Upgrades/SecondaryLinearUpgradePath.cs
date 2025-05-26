using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Upgrades/Secondary Linear Upgrade Path", fileName = "Secondary Linear Upgrade Path")]
    public class SecondaryLinearUpgradePath : LinearUpgradePath
    {
        [SerializeReferenceListContainer("modules", false, "Upgrade", "Upgrades")]
        [SerializeField] private UpgradeModulesContainer upgradesContainer;

        [Tooltip("The number of the step on the main upgrades path that this path will start appearing at.")]
        [SerializeField] private int startStepNumber = 5;
        [Tooltip("Every x steps after the first appearance, this upgrade path will appear.")]
        [SerializeField] private int stepsInterval = 5;

        protected override List<UpgradeModule> InitializeUpgradesList()
        {
            return upgradesContainer.Modules;
        }

        protected override void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}