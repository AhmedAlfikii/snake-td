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
    public abstract class ScriptableVariable<T> : ScriptableObject, IResettable
    {
        [Header("Value Controls")]
        [Tooltip("The value that the float will start with if it was not saved with a different value.")]
        [SerializeField] protected T defaultValue;

        [Header("Saving/Loading")]
        [Tooltip("The id used when saving to PlayerPrefs, if the ID is empty, the ScriptableObject name will be used instead.")]
        [SerializeField] protected string id;
        [Tooltip("Should the float automatically be saved whenever a change was made to it? If not, you'll have to manually call Save().")]
        [SerializeField] protected bool autoSave = true;
        [SerializeField] protected bool migrateFromOldId;
        [SerializeField] protected string oldId;

        [Header("Events")]
        [Tooltip("Fires whenever the value changes.")]
        [SerializeField] protected UnityEvent<T> onValueChange;
        [SerializeField] protected UnityEvent onValueChangeVoid;
        [SerializeField] protected UnityEvent<ScriptableVariable<T>> onValueChangeSelf;
        [SerializeField] protected UnityEvent<T> onDisplayValueChange;

        [NonSerialized] protected T currentValue;
        [NonSerialized] protected T currentDisplayValue;
        [NonSerialized] protected bool isInitialized;

        public T Value 
        {
            get 
            {
                if (!isInitialized)
                    Initialize();

                return currentValue; 
            }
        }
        public T DisplayValue 
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return currentDisplayValue;
            }
        }
        public T DefaultValue => defaultValue;
        public string ID 
        { 
            get
            {
                if (!string.IsNullOrEmpty(id))
                    return id;

                return name;
            }
        }

        public UnityEvent<T> OnValueChange => onValueChange;
        public UnityEvent<ScriptableVariable<T>> OnValueChangeSelf => onValueChangeSelf;
        public UnityEvent<T> OnDisplayValueChange => onDisplayValueChange;

        #region ScriptableObject Messages
        protected virtual void OnEnable()
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            if (!isInitialized)
                Initialize();
        }
        #endregion

        #region Private Functions
        protected void Initialize()
        {
            currentValue = GetSavedValue();
            currentDisplayValue = currentValue;

            isInitialized = true;

            OnInitialized();
        }
        protected void SetDisplayValue(T value)
        {
            currentDisplayValue = value;
            onDisplayValueChange?.Invoke(currentDisplayValue);
        }
        /// <summary>
        /// Gets called when a value is set, before saving it or invoking the value change event.
        /// Do your value processing here, for example, clamp it if it's a number, make sure it doesn't get below 0, etc...
        /// </summary>
        protected virtual void ProcessValueBeforeSet() { }
        #endregion

        #region Public Functions
        /// <summary>
        /// Save the value to PlayerPrefs (don't need to call this if you have "autoSave" enabled). 
        /// </summary>
        public void Save()
        {
            if (!isInitialized)
                Initialize();

            OnSavedValue();
        }
        /// <summary>
        /// Sets the value to the required value, save it if auto save is enabled, and invoke the on value change event.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hidden"></param>
        public void Set(T value, bool hidden = false)
        {
            if (!isInitialized)
                Initialize();

            currentValue = value;

            ProcessValueBeforeSet();

            if (!hidden)
                SetDisplayValue(value);

            if (autoSave)
                Save();

            onValueChange?.Invoke(currentValue);
            onValueChangeSelf?.Invoke(this);
            onValueChangeVoid?.Invoke();
        }
        public void ResetSavedData()
        {
            currentValue = defaultValue;
            SetDisplayValue(currentValue);

            Save();

            //onValueChange?.Invoke(currentValue);
        }
        #endregion

        protected abstract T GetSavedValue();
        protected abstract void OnSavedValue();
        protected virtual void OnInitialized() { }
    }
}