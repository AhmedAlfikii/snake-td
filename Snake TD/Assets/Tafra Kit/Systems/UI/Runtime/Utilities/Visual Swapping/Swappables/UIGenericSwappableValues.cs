using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.UI
{
    [System.Serializable]
    public abstract class UIGenericSwappableValues<T, V> : UISwappableValues
    {
        [SerializeField] protected T target;
        [SerializeField] protected List<V> stateValues;

        public List<V> StateValues => stateValues;

        public override void SetState(int stateIndex)
        {
            if(target == null)
                return;

            OnStateChange(stateIndex);
        }
    }
}