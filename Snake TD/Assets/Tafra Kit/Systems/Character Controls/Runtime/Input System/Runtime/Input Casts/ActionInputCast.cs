#if TAFRA_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TafraKit.InputSystem
{
    [CreateAssetMenu(fileName = "Action Input Cast", menuName = "Tafra Kit/Input/Input Casts/Action Input", order = 0)]
    public class ActionInputCast : InputCast
    {
        [SerializeField] private UnityEvent performed;

        public UnityEvent Performed => performed;

        public override void OnPerformed(InputAction.CallbackContext context)
        {
            performed?.Invoke();
        }

        public override void OnPerformed(InputAction inputAction)
        {
            performed?.Invoke();
        }
    }
}
#endif