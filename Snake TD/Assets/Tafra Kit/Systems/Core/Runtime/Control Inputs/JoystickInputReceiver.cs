using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class JoystickInputReceiver : MonoBehaviour
    {
        [SerializeField] private Object[] inputRecieverObjects;

        public UnityEvent<Vector3> OnJoystickInput = new UnityEvent<Vector3>();

        #region Private Fields
        private Joystick joystick;
        private List<IVector3InputReceiver> inputRecievers = new List<IVector3InputReceiver>();
        #endregion

        #region MonoBehaviour Messages
        private void Awake()
        {
            joystick = FindAnyObjectByType<Joystick>();
            for (int i = 0; i < inputRecieverObjects.Length; i++)
            {
                if (inputRecieverObjects[i] != null)
                {
                    IVector3InputReceiver ir = inputRecieverObjects[i] as IVector3InputReceiver;

                    if (ir != null)
                        inputRecievers.Add(ir);
                }
            }
        }
        private void OnEnable()
        {
            if (joystick != null)
                joystick.OnValueChange += OnValueChanged;
        }
        private void OnDisable()
        {
            if (joystick != null)
                joystick.OnValueChange -= OnValueChanged;
        }
        #endregion

        #region Callbacks
        private void OnValueChanged(Vector3 value)
        {
            OnJoystickInput?.Invoke(value);

            for (int i = 0; i < inputRecievers.Count; i++)
            {
                inputRecievers[i].RecieveInput(value);
            }
        }
        #endregion
    }
}