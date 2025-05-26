#if TAFRA_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TafraKit.InputSystem
{
    public abstract class InputCast : ScriptableObject
    {
        public string actionName;
        public bool alwaysInvoke;

        public virtual void OnCasterEnabled() { }
        public virtual void OnCasterDisabled() { }
        public abstract void OnPerformed(InputAction.CallbackContext context);
        public abstract void OnPerformed(InputAction inputAction);
    }
}
#endif