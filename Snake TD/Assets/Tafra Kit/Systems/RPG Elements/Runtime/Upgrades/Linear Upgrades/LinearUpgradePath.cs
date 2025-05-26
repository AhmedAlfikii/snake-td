using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Consumables;
using TafraKit.Internal;
using TafraKit.Internal.AI3;
using TafraKit.Mathematics;
using UnityEngine;

namespace TafraKit.RPG
{
    public abstract class LinearUpgradePath : ScriptableObject
    {
        [SerializeField] protected string id;

        [Header("Cost")]
        [SerializeField] protected Consumable costCurrency;
        [SerializeField] protected FormulasContainer costPerUpgradeNumber;

        [NonSerialized] protected List<UpgradeModule> upgrades;
        [NonSerialized] protected int upgradesCount;
        [NonSerialized] protected int appliedUpgradesCount;
        [NonSerialized] protected string appliedUpgradesCountSaveKey;

        public string ID => id;
        public List<UpgradeModule> Upgrades => upgrades;
        public Consumable CostCurrency => costCurrency;
        public int AppliedUpgradesCount => appliedUpgradesCount;

        private void LoadCore()
        {
            Load();

            OnLoaded();
        }
        private IEnumerator LoadCoreDelayed()
        {
            //The first frame of the game is always 0. And in frame 0, WaitForEndOfFrame actually skips a frame then waits for the end of that new frame.
            //So, to guarantee that the upgrades will be applied at the end of frame 1, we will skip frame 0.
            if(Time.frameCount == 0)
                yield return null;

            //We want to wait for the end of the frame to make sure that the player equipment were equipped, since this happens on start, and some upgrades depend on equipment.
            yield return Yielders.EndOfFrame;

            LoadCore();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnLoaded() { }
        protected abstract void Load();
        protected abstract List<UpgradeModule> InitializeUpgradesList();

        public void Initialize()
        {
            if(string.IsNullOrEmpty(id))
                TafraDebugger.Log("Linear Upgrade Path", "Linear upgrade path ID can't be null.", TafraDebugger.LogType.Error, this);

            appliedUpgradesCountSaveKey = $"UPGRADE_PATH_{id}_APPLIED_UPGRADES_COUNT";

            upgrades = InitializeUpgradesList();
            upgradesCount = upgrades.Count;

            OnInitialize();

            appliedUpgradesCount = TafraSaveSystem.LoadInt(appliedUpgradesCountSaveKey);

            GeneralCoroutinePlayer.StartCoroutine(LoadCoreDelayed());
        }
        public void ApplyUpgrade(int upgradeIndex)
        {
            if(upgradeIndex != appliedUpgradesCount)
            {
                TafraDebugger.Log("Linear Upgrade Path", $"Can't apply an upgrade that's out of turn." +
                    $"You're trying to apply an upgrade with the index of {upgradeIndex}, while you should be applying {appliedUpgradesCount}.", TafraDebugger.LogType.Error, this);
            }

            UpgradeModule targetUpgrade = upgrades[upgradeIndex];

            targetUpgrade.Apply();

            appliedUpgradesCount++;
            TafraSaveSystem.SaveInt(appliedUpgradesCountSaveKey, appliedUpgradesCount);
        }
        public void SceneLoaded()
        {
            for (int i = 0; i < appliedUpgradesCount; i++)
            {
                var upgrade = upgrades[i];

                upgrade.SceneLoaded();
            }
        }

        public float GetUpgradeCost(int upgradeIndex)
        { 
            return costPerUpgradeNumber.Evaluate(upgradeIndex + 1);
        }
    }
}