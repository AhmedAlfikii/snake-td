using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    [CreateAssetMenu(menuName = "Tafra Kit/Scriptables/Float", fileName = "Scriptable Float")]
    public class ScriptableFloat : ScriptableVariable<float>
    {
        #region Protected Serialized Fields
        [Header("Float Value Controls")]
        [Tooltip("Should the float go below 0?")]
        [SerializeField] protected bool allowNegativeValues;
        [SerializeField] protected bool capped;
        [SerializeField] protected TafraFloat cap;
        [SerializeField] protected UnityEvent<float> onValueAdd;
        [SerializeField] protected UnityEvent<float> onValueDeduct;
        #endregion

        #region Public Events
        public UnityEvent<float> OnValueAdd => onValueAdd;
        public UnityEvent<float> OnValueDeduct => onValueDeduct;
        #endregion

        /// <summary>
        /// Returns the current value of the float rounded to the closest int.
        /// </summary>
        public int ValueInt
        {
            get
            {
                return Mathf.RoundToInt(Value);
            }
        }
        /// <summary>
        /// Returns the current display value of the float rounded to the closest int.
        /// </summary>
        public int DisplayValueInt
        {
            get
            {
                return Mathf.RoundToInt(DisplayValue);
            }
        }

        #region Public Functions
        /// <summary>
        /// Adds a value to the float.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hidden"></param>
        /// <returns></returns>
        public virtual bool Add(float value,bool hidden = false)
        {
            if (!isInitialized)
                Initialize();

            float addedValue = value;
            float newValue = currentValue + addedValue;

            //If the value exceeded the cap, then clamp it.
            if (capped && newValue > cap.Value)
            {
                addedValue = cap.Value - currentValue;
                newValue = cap.Value;
            }

            currentValue = newValue;

            Set(currentValue, hidden);

            if (autoSave)
                Save();

            onValueAdd?.Invoke(addedValue);

            return true;
        }
        /// <summary>
        /// Adds a value to the float, only use this in inspector callbacks. Use Add(float, bool) in code instead since it doesn't have the extra function call this one does.
        /// </summary>
        /// <param name="value"></param>
        public void GenericAdd(float value)
        { 
            Add(value, false);
        }
        /// <summary>
        /// Deducts a value from the float and returns whether or not the deduction was successful 
        /// (the deduction will fail if "allowNegativeValues" is set to false and the float's value is less then the deducted value,
        /// otherwise it will return true).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Deduct(float value,bool hidden = false)
        {
            if (!isInitialized)
                Initialize();

            float newValue = currentValue - value;

            if (newValue < 0 && !allowNegativeValues)
                return false;

            currentValue = newValue;

            Set(newValue, hidden);

            onValueDeduct?.Invoke(value);
            
            return true;
        }
        /// <summary>
        /// Sets the float to the required value.
        /// </summary>
        /// <param name="value"></param>
        public void Set(float value)
        {
            Set(value, false);
        }

        /// <summary>
        /// Returns whether or not this float is capped.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCapped()
        {
            return capped;
        }

        /// <summary>
        /// Returns the cap of this float if enabled, if disabled, it will return 0.
        /// </summary>
        /// <returns></returns>
        public virtual float GetCap()
        {
            if (!capped) 
                return 0;

            return cap.Value;
        }

        public virtual void SetCap(float value)
        {
            if (cap.ScriptableVariable != null)
                cap.ScriptableVariable.Set(value);
            else
                cap.Value = value;
        }
        #endregion

        #region Protected Functions
        protected override void ProcessValueBeforeSet()
        {
            if (currentValue < 0 && !allowNegativeValues)
                currentValue = 0;

            if (capped && currentValue > cap.Value)
                currentValue = cap.Value;
        }
        protected override float GetSavedValue()
        {
            if(migrateFromOldId && PlayerPrefs.HasKey(oldId + "_FLOAT_SV_VALUE"))
            {
                PlayerPrefs.SetFloat(ID + "_FLOAT_SV_VALUE", PlayerPrefs.GetFloat(oldId + "_FLOAT_SV_VALUE"));
                PlayerPrefs.DeleteKey(oldId + "_FLOAT_SV_VALUE");
            }

            return PlayerPrefs.GetFloat(ID + "_FLOAT_SV_VALUE", defaultValue);
        }
        protected override void OnSavedValue()
        {
            PlayerPrefs.SetFloat(ID + "_FLOAT_SV_VALUE", currentValue);
        }
        #endregion

        #region Context Functions
        [ContextMenu("Add One")]
        private void CONTEXTAddOne()
        {
            Add(1);
        }
        [ContextMenu("Deduct One")]
        private void CONTEXTDeductOne()
        {
            Deduct(1);
        }
        [ContextMenu("Reset Value")]
        private void CONTEXTReset()
        {
            Deduct(Value);
        }
        #endregion
    }
}