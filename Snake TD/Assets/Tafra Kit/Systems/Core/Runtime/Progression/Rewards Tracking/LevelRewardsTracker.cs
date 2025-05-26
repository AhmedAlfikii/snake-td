using System;
using System.Collections.Generic;
using TafraKit.Consumables;
using UnityEngine;

namespace TafraKit.Internal
{
    //TODO: Also track inventories/storables.
    public class LevelRewardsTracker : MonoBehaviour, IResettable
    {
        public class ConsumableTracker
        {
            private Consumable consumable;
            private float startValue;
            private float change;

            private string consumableInitialValueSaveId;

            public Consumable Consumable => consumable;
            public float ChangeValue => change;

            public ConsumableTracker(Consumable consumable)
            { 
                this.consumable = consumable;

                consumableInitialValueSaveId = $"{consumable.ID}_LRT_START_VALUE";

                if(!TafraSaveSystem.HasKey(consumableInitialValueSaveId))
                {
                    TafraSaveSystem.SaveFloat(consumableInitialValueSaveId, consumable.Value);
                    startValue = consumable.Value;
                }
                else
                    startValue = TafraSaveSystem.LoadFloat(consumableInitialValueSaveId);

                change = consumable.Value - startValue;
            }

            public void OnEnable()
            { 
                consumable.OnValueChange.AddListener(OnValueChange);
            }
            public void OnDisable()
            {
                consumable.OnValueChange.RemoveListener(OnValueChange);
            }

            private void OnValueChange(float newValue)
            {
                change = newValue - startValue;
            }

            public void ResetSavedData()
            {
                TafraSaveSystem.DeleteKey(consumableInitialValueSaveId);
            }
        }

        [SerializeField] private List<Consumable> trackedConsumables = new List<Consumable>();

        private List<ConsumableTracker> consumableTrackers = new List<ConsumableTracker>();
        private Dictionary<Consumable, ConsumableTracker> trackerByConsumable = new Dictionary<Consumable, ConsumableTracker>();

        public List<ConsumableTracker> ConsumableTrackers => consumableTrackers;

        private void Awake()
        {
            for (int i = 0; i < trackedConsumables.Count; i++)
            {
                Consumable consumable = trackedConsumables[i];
                ConsumableTracker tracker = new ConsumableTracker(consumable);

                consumableTrackers.Add(tracker);

                trackerByConsumable.Add(consumable, tracker);
            }
        }
        private void OnEnable()
        {
            for(int i = 0; i < consumableTrackers.Count; i++)
            {
                ConsumableTracker tracker = consumableTrackers[i];

                tracker.OnEnable();
            }

            ProgressManager.OnLevelConcluded.AddListener(OnLevelConcluded);
        }
        private void OnDisable()
        {
            for(int i = 0; i < consumableTrackers.Count; i++)
            {
                ConsumableTracker tracker = consumableTrackers[i];

                tracker.OnDisable();
            }

            ProgressManager.OnLevelConcluded.RemoveListener(OnLevelConcluded);
        }

        private void OnLevelConcluded(GameLevel level, int levelNumber, LevelEndState state)
        {
            ResetSavedData();
        }

        public bool GetConsumableChangeValue(Consumable consumable, out float rewardValue)
        {
            if(trackerByConsumable.TryGetValue(consumable, out var tracker))
            {
                rewardValue = tracker.ChangeValue;
                return true;
            }
            else
            {
                rewardValue = 0f;
                return false;
            }
        }
        public void ResetSavedData()
        {
            for(int i = 0; i < consumableTrackers.Count; i++)
            {
                ConsumableTracker tracker = consumableTrackers[i];

                tracker.ResetSavedData();
            }
        }
    }
}