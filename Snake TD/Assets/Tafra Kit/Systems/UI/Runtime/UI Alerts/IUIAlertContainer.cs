using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.UI
{
    public interface IUIAlertContainer
    {
        public UIAlertState CurrentAlertState { get; }
        public UnityEvent<UIAlertState> OnAlertStateChange { get; }
    }
}