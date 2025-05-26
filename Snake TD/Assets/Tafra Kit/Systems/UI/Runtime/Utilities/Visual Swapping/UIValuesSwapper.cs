using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    public class UIValuesSwapper : MonoBehaviour
    {
        [SerializeReferenceListContainer("swappableValues", false, "Swappable", "Swappables")]
        [SerializeField] private UISwappableValuesContainer swappableValuesContainer;

        public void SetState(int stateIndex)
        {
            for(int i = 0; i < swappableValuesContainer.SwappableValues.Length; i++)
            {
                swappableValuesContainer.SwappableValues[i].SetState(stateIndex);
            }
        }

        public T GetModule<T>() where T : UISwappableValues
        {
            var swappableModules = swappableValuesContainer.SwappableValues;
            for (int i = 0; i < swappableModules.Length; i++)
            {
                var module = swappableModules[i];

                if(module.GetType() == typeof(T))
                {
                    return module as T;
                }
            }

            return null;
        }

        [ContextMenu("Set False")]
        public void SetFalse()
        {
            SetState(0);
        }
        [ContextMenu("Set True")]
        public void SetTrue()
        {
            SetState(1);
        }
    }
}