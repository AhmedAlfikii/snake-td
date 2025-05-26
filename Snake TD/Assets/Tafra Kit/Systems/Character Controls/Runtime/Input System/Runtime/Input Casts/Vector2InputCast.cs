#if TAFRA_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TafraKit;
using UnityEngine.Events;

namespace TafraKit.InputSystem
{
    [CreateAssetMenu(fileName = "Vector2 Input Cast", menuName = "Tafra Kit/Input/Input Casts/Vector2 Input", order = 0)]
    public class Vector2InputCast : InputCast
    {
        public UnityEvent<Vector2> performed = new UnityEvent<Vector2>();

        public override void OnPerformed(InputAction.CallbackContext context)
        {
            performed?.Invoke(context.ReadValue<Vector2>());
        }

        public override void OnPerformed(InputAction inputAction)
        {
            performed?.Invoke(inputAction.ReadValue<Vector2>());
        }
    }
}
#endif