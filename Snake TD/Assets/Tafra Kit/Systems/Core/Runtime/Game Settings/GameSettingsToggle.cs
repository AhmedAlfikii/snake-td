using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;
using TafraKit.UI;
using System;

namespace TafraKit.Internal
{
    public abstract class GameSettingsToggle : GameSettingsSection
    {
        [SerializeField] protected ToggleWithMotion toggleWithMotion;
         
        protected bool isOn;
        protected abstract bool IsManagerOn { get; }

        private void Awake()
        {
            isOn = IsManagerOn;

            toggleWithMotion.SetValue(isOn, true);
        }
        private void OnEnable()
        {
            if (toggleWithMotion != null)
            {
                toggleWithMotion.OnValueChanged.AddListener(SetValue);
            }
        }
        private void OnDisable()
        {
            if (toggleWithMotion != null)
            {
                toggleWithMotion.OnValueChanged.RemoveListener(SetValue);
            }
        }
        private void SetValue(bool on)
        {
            isOn = on;

            OnValueChange(on);
        }

        protected abstract void OnValueChange(bool on);
    }
}