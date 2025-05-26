#if TAFRA_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TafraKit.InputSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputCaster : MonoBehaviour
    {
        [System.Serializable]
        public class InputActionEvent : UnityEvent<InputAction.CallbackContext> { }

        [SerializeField] private InputCast[] inputCasts;

        private PlayerInput playerInput;
        private List<InputCast> alwaysUpdatedInputCasts = new List<InputCast>();

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            for(int i = 0; i < inputCasts.Length; i++)
            {
                InputCast inputCast = inputCasts[i];
                InputAction inputAction = playerInput.actions[inputCast.actionName];

                if(inputAction != null && inputCast.alwaysInvoke)
                    alwaysUpdatedInputCasts.Add(inputCast);
            }

        }
        private void OnEnable()
        {
            for(int i = 0; i < inputCasts.Length; i++)
            {
                InputCast inputCast = inputCasts[i];
                InputAction inputAction = playerInput.actions[inputCast.actionName];

                if(inputAction != null)
                {
                    inputCast.OnCasterEnabled();

                    if(!inputCast.alwaysInvoke)
                        inputAction.performed += inputCasts[i].OnPerformed;
                }
            }
        }
        private void OnDisable()
        {
            for(int i = 0; i < inputCasts.Length; i++)
            {
                InputCast inputCast = inputCasts[i];
                InputAction inputAction = playerInput.actions[inputCast.actionName];

                if(inputAction != null)
                {
                    inputCast.OnCasterDisabled();

                    if(!inputCast.alwaysInvoke)
                        inputAction.performed -= inputCasts[i].OnPerformed;
                }
            }

        }
        private void Update()
        {
            for(int i = 0; i < alwaysUpdatedInputCasts.Count; i++)
            {
                InputCast inputCast = alwaysUpdatedInputCasts[i];
                InputAction inputAction = playerInput.actions[inputCast.actionName];

                inputCast.OnPerformed(inputAction);
            }
        }
    }
}
#endif