using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    [Serializable]
    public abstract class Condition
    {
        [NonSerialized] private Action onSatisfied;
        [NonSerialized] protected bool isInitialized;
        [NonSerialized] protected bool isActive;
        [NonSerialized] protected bool isSatisfied;
        [NonSerialized] protected bool checkPerformedOnActivation;

        public bool IsActive => isActive;
        public Action OnSatisfied { get { return onSatisfied; } set { onSatisfied = value; } }


        public void Activate(bool performCheck = true)
        {
            if (!isInitialized)
                Initialize();

            isActive = true;
            isSatisfied = false;
            checkPerformedOnActivation = performCheck;

            OnActivate();

            if(!isActive) return;

            if (performCheck)
                Check();
        }
        public void Deactivate()
        {
            isActive = false;
            OnDeactivate();
        }

        private void Initialize()
        {
            isInitialized = true;

            OnInitialize();
        }
        protected void Satisfy()
        {
            if(isSatisfied)
                return;

            isSatisfied = true;

            OnSatisfy();

            onSatisfied?.Invoke();

            Deactivate();
        }
        protected virtual void OnInitialize() { }
        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }
        protected virtual void OnSatisfy() { }
        protected abstract bool PerformCheck();

        /// <summary>
        /// Check whether or not the condition should be satisfied, and satisfy it if it should. 
        /// </summary>
        public bool Check() 
        {
            if(!isInitialized)
            {
                Debug.LogError("Condition isn't initialized, please activate it before performing a check.");
                return false;
            }

            bool satisfied = PerformCheck();

            if(satisfied)
                Satisfy();
            else
                isSatisfied = false;

            return satisfied;
        }
        /// <summary>
        /// Returns whether or not the condition was satisfied before (doesn't perform any checks).
        /// </summary>
        /// <returns></returns>
        public bool WasSatisfied()
        {
            return isSatisfied;
        }
    }
}