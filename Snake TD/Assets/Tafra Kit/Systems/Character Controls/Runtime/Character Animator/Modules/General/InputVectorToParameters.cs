using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.InputSystem;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("General/Input Vector To Parameters")]
    public class InputVectorToParameters : CharacterAnimatorModule
    {
        [SerializeField] private string movementInputParameter = "Movement Input";
        #if TAFRA_INPUT_SYSTEM
        [SerializeField] private Vector2InputCast inputCaster;
        #endif
        [NonSerialized] private Transform transform;
        [NonSerialized] private Vector2 input;
        [NonSerialized] private int movementInputParameterHash;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => true;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            transform = characterAnimator.transform;

            movementInputParameterHash = Animator.StringToHash(movementInputParameter);
        }

        protected override void OnEnable()
        {
           #if TAFRA_INPUT_SYSTEM
            inputCaster.performed.AddListener(OnInputReceived);
            #endif
        }
        protected override void OnDisable()
        {
            #if TAFRA_INPUT_SYSTEM
            inputCaster.performed.RemoveListener(OnInputReceived);
            #endif
        }
        
        public override void LateUpdate()
        {
            float inputMagnitude = input.magnitude;
            
            animator.SetFloat(movementInputParameterHash, inputMagnitude);
        }

        private void OnInputReceived(Vector2 input)
        {
            this.input = input;
        }
    }
}