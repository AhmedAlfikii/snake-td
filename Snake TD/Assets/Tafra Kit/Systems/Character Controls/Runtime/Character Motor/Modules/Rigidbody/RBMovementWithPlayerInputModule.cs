using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.InputSystem;
using UnityEngine.Serialization;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Rigidbody/Movement - Player Input (RB)")]
    public class RBMovementWithPlayerInputModule : CharacterMotorModule
    {
        #if TAFRA_INPUT_SYSTEM
        [FormerlySerializedAs("inputCaster")]
        [SerializeField] private Vector2InputCast inputCast;
        #endif

        private RigidbodyMotor rbMotor;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            rbMotor = motor as RigidbodyMotor;

            if (rbMotor == null ) 
                TafraDebugger.Log("Movement With Player Input Module", "\"Movement With Player Input Module\" only works with RigidbodyMotors.", TafraDebugger.LogType.Error);
        }
        protected override void OnEnable()
        {
           #if TAFRA_INPUT_SYSTEM
            inputCast.performed.AddListener(OnInputReceived);
            #endif
        }
        protected override void OnDisable()
        {
            #if TAFRA_INPUT_SYSTEM
            inputCast.performed.RemoveListener(OnInputReceived);
            #endif
        }

        private void OnInputReceived(Vector2 input)
        {
            Vector3 dir = new Vector3(input.x, 0, input.y);

            rbMotor.InputMotionVector = dir;
            rbMotor.RawInputMotionVector = dir;
        }
    }
}