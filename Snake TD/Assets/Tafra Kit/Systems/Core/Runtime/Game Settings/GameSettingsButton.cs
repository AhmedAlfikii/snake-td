using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.UI;
using System;

namespace TafraKit.Internal
{
    [RequireComponent(typeof(ZButton))]
    public abstract class GameSettingsButton : GameSettingsSection
    {
        private ZButton button;

        protected virtual void Awake()
        {
            button = GetComponent<ZButton>();

            button.onClick.AddListener(OnClick);
        }

        protected abstract void OnClick();
    }
}