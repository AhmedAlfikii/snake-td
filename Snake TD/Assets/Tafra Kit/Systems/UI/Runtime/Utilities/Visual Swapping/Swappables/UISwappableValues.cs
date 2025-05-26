using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.UI
{
    [System.Serializable]
    public abstract class UISwappableValues
    {
        /// <summary>
        /// Changes the state of the UI, in case of on/off scenarios, use state 0 as off and state 1 as on.
        /// </summary>
        /// <param name="stateIndex"></param>
        public virtual void SetState(int stateIndex)
        { 
            OnStateChange(stateIndex);
        }
        protected abstract void OnStateChange(int stateIndex);
    }
}