using System;
using System.Collections.Generic;
using TafraKit.ContentManagement;
using TafraKit.Internal.RPGElements;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.RPG
{
    [Serializable]
    public class StatsContainer
    {
        [SerializeField] private List<TafraAsset<ScriptableObject>> baseStatObjects = new List<TafraAsset<ScriptableObject>>();
        [SerializeField] private List<StatCollectionAccessories> statCollectionAccessories = new List<StatCollectionAccessories>();

        [SerializeField] private UnityEvent<StatCollection> onStatAdded = new UnityEvent<StatCollection>();
        [SerializeField] private UnityEvent<Stat> onStatRemoved = new UnityEvent<Stat>();
        /// <summary>
        /// Gets fired when a stat changes or when a stat is newly added.
        /// </summary>
        [SerializeField] private UnityEvent<Stat, float> onStatUpdated = new UnityEvent<Stat, float>();

        [NonSerialized] private bool isInitialized;
        [NonSerialized] private bool isActive;
        [NonSerialized] private Dictionary<Stat, StatCollection> statCollectionByStat = new Dictionary<Stat, StatCollection>();
        [NonSerialized] private Dictionary<Stat, StatCollectionAccessories> statCollectionAccessoriesByStat = new Dictionary<Stat, StatCollectionAccessories>();
        [NonSerialized] private List<StatCollection> statCollections = new List<StatCollection>();
        [NonSerialized] private List<IStatValuesList> baseStats = new List<IStatValuesList>();

        public UnityEvent<StatCollection> OnStatAdded => onStatAdded;
        public UnityEvent<Stat> OnStatRemoved => onStatRemoved;
        /// <summary>
        /// Gets fired when a stat changes or when a stat is newly added.
        /// </summary>
        public UnityEvent<Stat, float> OnStatUpdated => onStatUpdated;
        public List<StatCollection> StatCollections => statCollections;

        public void Initialize(bool autoActivate = true)
        {
            if(isInitialized)
                return;

            for (int i = 0; i < statCollectionAccessories.Count; i++)
            {
                var accessories = statCollectionAccessories[i];

                statCollectionAccessoriesByStat.Add(accessories.Stat.Load(), accessories);
            }

            isInitialized = true;

            for(int i = 0; i < baseStatObjects.Count; i++)
            {
                if(baseStatObjects[i].Load() is IStatValuesList statContainer)
                {
                    baseStats.Add(statContainer);
                    statContainer.InitializeStatValues();
                }
            }

            if(autoActivate)
                Activate();
        }

        public void Activate()
        {
            if (isActive) 
                return;

            for(int i = 0; i < baseStats.Count; i++)
            {
                AddStatValues(baseStats[i].StatValues);
            }

            isActive = true;
        }
        public void Deactivate()
        {
            for (int i = 0; i < statCollections.Count; i++)
            {
                statCollections[i].RemoveAllStatValues();
            }
            statCollectionByStat.Clear();
            statCollections.Clear();

            isActive = false;
        }
        public void AddStatValue(StatValue statValue)
        {
            if(statValue == null)
                return;
            
            StatCollection collection = null;

            if(statCollectionByStat.TryGetValue(statValue.Stat, out collection))
            {
                statCollectionByStat[statValue.Stat].AddStatValue(statValue);
            }
            else
            {
                StatCollectionAccessories accessories = null;

                statCollectionAccessoriesByStat.TryGetValue(statValue.Stat, out accessories);

                collection = new  StatCollection(statValue.Stat, accessories, OnStatCollectionValueChange);

                collection.AddStatValue(statValue);

                statCollectionByStat.Add(statValue.Stat, collection);
                statCollections.Add(collection);

                onStatAdded.Invoke(collection);
            }

            onStatUpdated.Invoke(statValue.Stat, collection.TotalValue);
        }
        public void AddStatValues(List<StatValue> statValues)
        {
            if(statValues == null || statValues.Count == 0)
                return;

            for(int i = 0; i < statValues.Count; i++)
            {
                AddStatValue(statValues[i]);
            }
        }
        public void RemoveStatValue(StatValue statValue)
        {
            if(statValue == null)
                return;

            Stat stat = statValue.Stat;
            if(statCollectionByStat.TryGetValue(stat, out StatCollection collection))
            {
                //If this is the only stat value that exists in this collection, then completely remove the collection.
                if(collection.StatValues.Count == 1 && collection.StatValues[0] == statValue)
                {
                    collection.Unload();
                    statCollections.Remove(collection);
                    statCollectionByStat.Remove(stat);
                    onStatRemoved.Invoke(stat);
                }
                else
                    collection.RemoveStatValue(statValue);
            }
        }
        public void RemoveStatValues(List<StatValue> statValues)
        {
            if(statValues == null || statValues.Count == 0)
                return;

            for(int i = 0; i < statValues.Count; i++)
            {
                RemoveStatValue(statValues[i]);
            }
        }
        public bool AddStatManipulator(Stat stat, ValueManipulator manipulator, bool recalculate = true)
        {
            if(statCollectionByStat.TryGetValue(stat, out var statCollection))
                return statCollection.AddManipulator(manipulator, recalculate);
            else
            { 
                StatValue sv = new StatValue(stat, new Mathematics.FormulasContainer());
                AddStatValue(sv);

                if(statCollectionByStat.TryGetValue(stat, out var newStatCollection))
                    return newStatCollection.AddManipulator(manipulator, recalculate);
            }

            return false;
        }
        public void RemoveStatManipulator(Stat stat, ValueManipulator manipulator, bool recalculate = true)
        {
            if(statCollectionByStat.TryGetValue(stat, out var statCollection))
                statCollection.RemoveManipulator(manipulator, recalculate);
        }
        /// <summary>
        /// Call this to force recalculate a stat's total value whenever you know that a manipulator value that can't be listened to was changed.
        /// </summary>
        /// <param name="stat"></param>
        public void ForceRecalculateStat(Stat stat)
        {
            if(statCollectionByStat.TryGetValue(stat, out var collection))
                collection.CalculateTotalValue();

        }
        public float GetStatTotalValue(Stat stat)
        {
            if(statCollectionByStat.TryGetValue(stat, out var collection))
                return collection.TotalValue;

            return 0;
        }
        public float GetStatBaseValue(Stat stat)
        {
            if(statCollectionByStat.TryGetValue(stat, out var collection))
                return collection.BaseValue;

            return 0;
        }
        public float GetStatExtraValue(Stat stat)
        {
            if(statCollectionByStat.TryGetValue(stat, out var collection))
                return collection.ExtraValue;

            return 0;
        }

        private void OnStatCollectionValueChange(StatCollection collection)
        {
            onStatUpdated.Invoke(collection.Stat, collection.TotalValue);
        }
    }
}