using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class ScriptableFloatMonitor : MonoBehaviour
    {
        [SerializeField] private ScriptableFloat scriptableFloat;

        public UnityEvent OnIsOne;
        //TODO
        public UnityEvent OnIsZero;
        //TODO
        public UnityEvent OnChanged;
        //TODO
        public UnityEvent OnIncreased;
        //TODO
        public UnityEvent OnDecreased;

        private void OnEnable()
        {
            scriptableFloat.OnValueChange.AddListener(OnValueChange);
            OnValueChange(scriptableFloat.Value);
        }
        private void OnDisable()
        {
            scriptableFloat.OnValueChange.RemoveListener(OnValueChange);
        }
        private void OnValueChange(float value)
        {
            int intValue = Mathf.RoundToInt(value);

            if(intValue == 1)
                OnIsOne?.Invoke();
        }
    }
}