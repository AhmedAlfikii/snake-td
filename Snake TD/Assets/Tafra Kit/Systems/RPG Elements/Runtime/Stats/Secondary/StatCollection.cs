using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.ContentManagement;
using TafraKit.RPG;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace TafraKit.Internal.RPGElements
{
    /// <summary>
    /// A collection that contains the total value of a group of stat values for a specific stat.
    /// </summary>
    [Serializable]
    public class StatCollection
    {
        #region Private Fields
        [NonSerialized] private List<StatValue> statValues;
        [NonSerialized] private Stat stat;
        [NonSerialized] private TafraAsset<ScriptableFloat> outputSFRef;
        [NonSerialized] private ScriptableFloat outputSF;
        [NonSerialized] private List<ValueManipulator> manipulators;
        /// <summary>
        /// The total value of the sum of the stat values multiplied by the multipliers and with the increasers added for both base stats and extra stats.
        /// </summary>
        [NonSerialized] private float totalValue;
        /// <summary>
        /// The value of the sum of the stat values before applying the extra stat values and manipulators.
        /// </summary>
        [NonSerialized] private float baseValue;
        /// <summary>
        /// The value that the extra stat values and manipulators have added to the base stat value.
        /// </summary>
        [NonSerialized] private float extraValue;
        /// <summary>
        /// The sum of all the multipliers that are over 1.
        /// </summary>
        [NonSerialized] private float totalBuffingMultipliers;
        /// <summary>
        /// The sum of all the multipliers that are below than 1.
        /// </summary>
        [NonSerialized] private float totalNerfingMultipliers;
        /// <summary>
        /// The sum of all the increasers in the manipulators.
        /// </summary>
        [NonSerialized] private float totalIncreasers;
        /// <summary>
        /// The sum of all the decreasers in the manipulators.
        /// </summary>
        [NonSerialized] private float totalDecreasers;
        [NonSerialized] private Action<StatCollection> onValueChanged;
        #endregion

        #region Public Properties
        /// <summary>
        /// The stat that this collection represents the value of.
        /// </summary>
        public Stat Stat => stat;
        /// <summary>
        /// The currently applied stat values.
        /// </summary>
        public List<StatValue> StatValues => statValues;
        /// <summary>
        /// The total value of the sum of the stat values multiplied by the multipliers and with the increasers added.
        /// </summary>
        public float TotalValue => totalValue;
        /// <summary>
        /// The value of the sum of the stat values before applying the extra stat values and manipulators.
        /// </summary>
        public float BaseValue => baseValue;
        /// <summary>
        /// The value that the extra stat values and manipulators have added to the base stat value.
        /// </summary>
        public float ExtraValue => extraValue;
        /// <summary>
        /// The sum of all the multipliers that are over 1.
        /// </summary>
        public float TotalBuffingMultipliers => totalBuffingMultipliers;
        /// <summary>
        /// The sum of all the multipliers that are below than 1.
        /// </summary>
        public float TotalNerfingMultipliers => totalNerfingMultipliers;
        /// <summary>
        /// The sum of all the increasers in the manipulators.
        /// </summary>
        public float TotalIncreasers => totalIncreasers;
        /// <summary>
        /// The sum of all the decreasers in the manipulators.
        /// </summary>
        public float TotalDecreasers => totalDecreasers;
        #endregion

        #region Constructors
        public StatCollection(Stat stat, Action<StatCollection> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            this.stat = stat;
            outputSF = null;

            manipulators = new List<ValueManipulator>();
            statValues = new List<StatValue>();
        }
        public StatCollection(Stat stat, StatCollectionAccessories accessories, Action<StatCollection> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            this.stat = stat;

            manipulators = new List<ValueManipulator>();

            statValues = new List<StatValue>();

            if(accessories != null)
            {
                outputSFRef = accessories.Output;
                outputSF = outputSFRef.Load();

                for (int i = 0; i < accessories.Manipulators.Count; i++)
                {
                    AddManipulator(accessories.Manipulators[i], false);
                }

                CalculateTotalValue();
            }
        }
        #endregion

        #region Callbacks
        private void OnManipulatorValueChange(float newValue)
        {
            CalculateTotalValue();
        }
        private void OnStatValueChanged()
        {
            CalculateTotalValue();
        }
        #endregion

        #region Private Function
        private void CalculateBaseValue()
        {
            baseValue = 0;

            for(int i = 0; i < statValues.Count; i++)
            {
                var statValue = statValues[i];
                
                if(statValue.IsExtra)
                    continue;

                baseValue += statValue.CurrentValue;
            }

            float buffMultiplicationValue = 0;
            float nerfMultiplicationValue = 0;
            float increasersValue = 0;
            float decreasersValue = 0;
            float overridingValue = 0;

            for(int i = 0; i < manipulators.Count; i++)
            {
                var manipulator = manipulators[i];

                if(manipulator.isExtra)
                    continue;

                switch(manipulator.operation)
                {
                    case NumberOperation.Add:
                        increasersValue += manipulator.value.Value;
                        break;
                    case NumberOperation.Subtract:
                        decreasersValue += manipulator.value.Value;
                        break;
                    case NumberOperation.Multiply:
                        {
                            var val = manipulator.value;

                            if(val.Value > 1)
                                buffMultiplicationValue += val.Value - 1;
                            else if(val.Value < 1)
                                nerfMultiplicationValue += 1 - val.Value;
                        }
                        break;
                    case NumberOperation.Divide:
                        {
                            var val = 1f / manipulator.value.Value;

                            if(val > 1)
                                buffMultiplicationValue += val - 1;
                            else if(val < 1)
                                nerfMultiplicationValue += 1 - val;
                        }
                        break;
                    case NumberOperation.Set:
                        overridingValue = manipulator.value.Value;
                        break;
                }
            }

            totalBuffingMultipliers = buffMultiplicationValue;
            totalNerfingMultipliers = nerfMultiplicationValue;
            totalIncreasers = increasersValue;
            totalDecreasers = decreasersValue;

            baseValue += baseValue * buffMultiplicationValue;
            baseValue -= baseValue * nerfMultiplicationValue;

            baseValue += increasersValue;
            baseValue -= decreasersValue;
        }
        #endregion

        #region Public Functions
        public void CalculateTotalValue()
        {
            float originalValue = totalValue;

            totalValue = 0;

            for(int i = 0; i < statValues.Count; i++)
            {
                var statValue = statValues[i];
                float val = statValue.CurrentValue;

                totalValue += val;
            }
            
            float buffMultiplicationValue = 0;
            float nerfMultiplicationValue = 0;
            float increasersValue = 0;
            float decreasersValue = 0;
            float overridingValue = 0;

            for (int i = 0; i < manipulators.Count; i++)
            {
                var manipulator = manipulators[i];

                switch(manipulator.operation)
                {
                    case NumberOperation.Add:
                        increasersValue += manipulator.value.Value;
                        break;
                    case NumberOperation.Subtract:
                        decreasersValue += manipulator.value.Value;
                        break;
                    case NumberOperation.Multiply:
                        {
                            var val = manipulator.value;

                            if(val.Value > 1)
                                buffMultiplicationValue += val.Value - 1;
                            else if(val.Value < 1)
                                nerfMultiplicationValue += 1 - val.Value;
                        }
                        break;
                    case NumberOperation.Divide:
                        {
                            var val = 1f / manipulator.value.Value;

                            if(val > 1)
                                buffMultiplicationValue += val - 1;
                            else if(val < 1)
                                nerfMultiplicationValue += 1 - val;
                        }
                        break;
                    case NumberOperation.Set:
                        overridingValue = manipulator.value.Value;
                        break;
                }
            }

            totalBuffingMultipliers = buffMultiplicationValue;
            totalNerfingMultipliers = nerfMultiplicationValue;
            totalIncreasers = increasersValue;
            totalDecreasers = decreasersValue;

            totalValue += totalValue * buffMultiplicationValue;
            totalValue -= totalValue * nerfMultiplicationValue;

            totalValue += increasersValue;
            totalValue -= decreasersValue;

            CalculateBaseValue();
            extraValue = totalValue - baseValue;

            //If no change was made in the total value, then don't invoke any events.
            float delta = totalValue - originalValue;
            if(Mathf.Abs(delta) < 0.0001f)
                return;

            if(outputSF != null)
                outputSF.Set(totalValue);

            onValueChanged?.Invoke(this);
        }
        public void AddStatValue(StatValue statValue)
        {
            if(statValue.Stat != stat || statValues.Contains(statValue))
                return;

            statValue.OnValueChange.AddListener(OnStatValueChanged);

            statValues.Add(statValue);

            CalculateTotalValue();
        }
        public void RemoveStatValue(StatValue statValue)
        {
            if(statValue.Stat != stat || !statValues.Contains(statValue))
                return;

            statValue.OnValueChange.RemoveListener(OnStatValueChanged);

            statValues.Remove(statValue);

            CalculateTotalValue();
        }
        public bool AddManipulator(ValueManipulator manipulator, bool recalculate = true)
        {
            if(!manipulators.Contains(manipulator))
            {
                manipulators.Add(manipulator);

                ScriptableFloat sf = manipulator.value.ScriptableVariable;

                if(sf != null)
                    sf.OnValueChange.AddListener(OnManipulatorValueChange);

                if (recalculate)
                    CalculateTotalValue();

                return true;
            }

            return false;
        }
        public void RemoveManipulator(ValueManipulator manipulator, bool recalculate = true)
        {
            if(manipulators.Remove(manipulator))
            {
                ScriptableFloat sf = manipulator.value.ScriptableVariable;

                if(sf != null)
                    sf.OnValueChange.RemoveListener(OnManipulatorValueChange);

                if (recalculate)
                    CalculateTotalValue();
            }
        }
        public void RemoveAllStatValues()
        {
            for (int i = 0; i < statValues.Count; i++)
            {
                var statValue = statValues[i];

                statValue.OnValueChange.RemoveListener(OnStatValueChanged);
            }

            statValues.Clear();
        }
        public void Unload()
        { 
            if (outputSFRef != null)
                outputSFRef.Release();
        }
        #endregion
    }
}