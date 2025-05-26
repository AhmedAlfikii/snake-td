using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.GraphViews
{
    [System.Serializable]
    public class ExposableProperty
    {
        [SerializeField, HideInInspector] public string name;
        [Tooltip("Should this property be exposed in the agent?")]
        public bool expose;
        public string tooltip;
        [SerializeField, HideInInspector] protected int id;

        [NonSerialized] private UnityEvent onValueChange;

        public UnityEvent OnValueChange => onValueChange;

        public int ID => id;

        public ExposableProperty()
        { 

        }
        public ExposableProperty(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
        public ExposableProperty(string name, string tooltip, int id)
        {
            this.name = name;
            this.tooltip = tooltip;
            this.id = id;
        }

        public void EnableValueChangeSignal()
        {
            onValueChange = new UnityEvent();
        }
        public void SignalValueChange()
        {
            onValueChange?.Invoke();
        }
    }
}