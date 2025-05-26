#if TAFRA_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TafraKit;

namespace TafraKit.InputSystem
{
    [CreateAssetMenu(fileName = "Float Input Cast", menuName = "Tafra Kit/Input/Input Casts/Float Input", order = 0)]
    public class FloatInputCast : InputCast
    {
        public FloatUnityEvent Event;

        public override void OnPerformed(InputAction.CallbackContext context)
        {
            Event?.Invoke(context.ReadValue<float>());
        }

        public override void OnPerformed(InputAction inputAction)
        {
            Event?.Invoke(inputAction.ReadValue<float>());
        }
    }
}
#endif