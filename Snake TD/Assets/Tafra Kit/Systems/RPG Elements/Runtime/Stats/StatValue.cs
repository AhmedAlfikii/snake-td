using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TafraKit.Mathematics;
using TafraKit.ContentManagement;

namespace TafraKit.RPG
{
    [Serializable]
    public class StatValue
    {
        [Tooltip("Serves no purpose but to clarify th stat name in an inspector list.")]
        [SerializeField] protected string name;
        [SerializeField] protected TafraAsset<Stat> stat;
        [Tooltip("If true, the value added to the stat by this element will be part of the \"Extra Value\", which is the value caused by stat values or manipulators that are not considered base." +
            "Will not change the results of the stat calculation.")]
        [SerializeField] protected bool isExtra;
        [SerializeField] protected FormulasContainer valueAtLevel;

        [NonSerialized] protected float currentValue;
        [NonSerialized] protected UnityEvent onValueChange = new UnityEvent();
        [NonSerialized] protected Stat statObject;

        public UnityEvent OnValueChange => onValueChange;
        public float CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                onValueChange?.Invoke();
            }
        }
        public Stat Stat 
        {
            get
            {
                if(statObject == null)
                    return stat.Load();

                return statObject;
            }
        }
        /// <summary>
        /// If true, the value added to the stat by this element will be part of the \"Extra Value\", which is the value caused by stat values or manipulators that are not considered base.
        /// </summary>
        public bool IsExtra => isExtra;

        public StatValue()
        { 
            onValueChange = new UnityEvent();
        }
        public StatValue(Stat stat, FormulasContainer valueAtLevel)
        {
            name = stat.DisplayName;
            this.valueAtLevel = valueAtLevel;
            onValueChange = new UnityEvent();
        }

        public float GetValueAt(int level)
        {
            return valueAtLevel.Evaluate(level);
        }
        public void SetLevel(int level)
        {
            CurrentValue = valueAtLevel.Evaluate(level);
        }
        public FormulasContainer GetUpgradeEquations()
        {
            return valueAtLevel;
        }
        public void SetUpgradeEquations(FormulasContainer upgradeEquations)
        {
            this.valueAtLevel = upgradeEquations;
        }
    }
}