#if TAFRA_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TafraKit.InputSystem
{
    /// <summary>
    /// Takes the input and makes it relative to the main camera's forward axis.
    /// </summary>
    [CreateAssetMenu(fileName = "Vector2 Input Cast", menuName = "Tafra Kit/Input/Input Casts/Vector2 Camera Input", order = 0)]
    public class Vector2CameraInputCast : Vector2InputCast
    {
        private Transform camTransform;

        public override void OnCasterEnabled()
        {
            camTransform = Camera.main.transform;    
        }

        public override void OnPerformed(InputAction.CallbackContext context)
        {
            ConvertAndPerform(context.ReadValue<Vector2>());
        }

        public override void OnPerformed(InputAction inputAction)
        {
            ConvertAndPerform(inputAction.ReadValue<Vector2>());
        }

        private void ConvertAndPerform(Vector2 originalInput)
        {
            Vector3 mappedValue = new Vector3(originalInput.x, 0, originalInput.y);

            Vector3 camForward = camTransform.forward;
            camForward.y = 0;   //ignore the Y (since we don't want the player to affect the motor's speed if the camera is tilting downwards/upwards).

            Quaternion quat = Quaternion.LookRotation(camForward, Vector3.up);

            //Change the direction to be relative to the camera's forward direction.
            Vector3 camRelativeDir = quat * mappedValue;

            Vector3 camRelativeInput = new Vector3(camRelativeDir.x, camRelativeDir.z);

            performed?.Invoke(camRelativeInput);
        }
    }
}
#endif