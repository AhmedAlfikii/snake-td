using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    public class LinearUpgradesSettings : SettingsModule
    {
        [SerializeField] private bool enabled;
        [SerializeField] private MainLinearUpgradePath mainUpgradePath;
        [SerializeField] private List<SecondaryLinearUpgradePath> secondaryUpgradePaths = new List<SecondaryLinearUpgradePath>();

        public MainLinearUpgradePath MainUpgradePath => mainUpgradePath;
        public List<SecondaryLinearUpgradePath> SecondaryUpgradePaths => secondaryUpgradePaths;
        public bool Enabled => enabled;
        public override string Name => "RPG/Upgrades/Linear Upgrades";
        public override string Description => "Control how players upgrade their power.";
    }
}