using System;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Upgrades/Linear Upgrade Path - Repeating", fileName = "Linear Upgrade Path")]
    public class MainLinearUpgradePathRepeating : MainLinearUpgradePath
    {
        [SerializeReferenceListContainer("modules", false, "Upgrade", "Upgrades")]
        [SerializeField] private RepeatingUpgradeModulesContainer upgradesContainer;

        [Tooltip("Will loop the list of upgrades until the count reaches this number.")]
        [SerializeField] private int repeatUntil = 200;

        [NonSerialized] private List<RepeatingUpgradeModule> repeatingUpgrades;
        [NonSerialized] private int repeatingUpgradesCount;

        protected override List<UpgradeModule> InitializeUpgradesList()
        {
            repeatingUpgrades = upgradesContainer.Modules;
            repeatingUpgradesCount = repeatingUpgrades.Count;

            List<UpgradeModule> upgradeModules = new List<UpgradeModule>();

            for (int i = 0; i < repeatUntil; i++)
            {
                int originalUpgradeIndex = i % repeatingUpgradesCount;
                int recurrenceNumber = (int)(i / repeatingUpgradesCount) + 1;

                RepeatingUpgradeModule originalUpgrade = repeatingUpgrades[originalUpgradeIndex];
                RepeatingUpgradeModule newModule = originalUpgrade.Clone(recurrenceNumber);
                
                newModule.Initialize(this);

                upgradeModules.Add(newModule);
            }

            return upgradeModules;
        }
        protected override void Load()
        {
            for (int i = 0; i < appliedUpgradesCount; i++)
            {
                if(upgrades.Count <= i)
                    break;

                var upgrade = upgrades[i];

                upgrade.LoadedApply();
            }
        }
    }
}