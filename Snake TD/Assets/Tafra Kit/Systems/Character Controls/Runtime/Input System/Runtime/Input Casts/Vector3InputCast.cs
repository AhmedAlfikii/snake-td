#if TAFRA_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TafraKit;

namespace TafraKit.InputSystem
{
    [CreateAssetMenu(fileName = "Vector3 Input Cast", menuName = "Tafra Kit/Input/Input Casts/Vector3 Input", order = 0)]
    public class Vector3InputCast : InputCast
    {
        public Vector3UnityEvent performed;

        public override void OnPerformed(InputAction.CallbackContext context)
        {
            performed?.Invoke(context.ReadValue<Vector3>());
        }

        public override void OnPerformed(InputAction inputAction)
        {
            performed?.Invoke(inputAction.ReadValue<Vector3>());
        }
    }
}
#endif