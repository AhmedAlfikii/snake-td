using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public abstract class ResettableScriptableObject : ScriptableObject
    {
        #region Events
        [NonSerialized] public UnityEvent OnSavedDataReset = new UnityEvent();
        #endregion
        
        #region Public Functions
        public void ResetData()
        {
            OnDataReset();
            
            OnSavedDataReset?.Invoke();
        }
        #endregion

        #region Protected Functions
        protected abstract void OnDataReset();
        #endregion
    }
}