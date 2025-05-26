using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.InputSystem;
using UnityEngine.Serialization;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Rigidbody/Slidey Movement - Player Input (RB)")]
    public class RBSlideyMovementWithPlayerInputModule : CharacterMotorModule
    {
        #if TAFRA_INPUT_SYSTEM
        [SerializeField] private Vector2InputCast inputCast;
        #endif

        [SerializeField] private float rotationCatchUpSpeed = 10;
        [SerializeField] private float anuglarVelocityUpSpeed = 250;
        [SerializeField] private float movementCatchUpSpeed = 10;

        private RigidbodyMotor rbMotor;
        private Rigidbody rigidbody;
        private Vector3 curInput;
        private Vector3 curLookDir;
        private Vector3 curMovementDir;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            rbMotor = motor as RigidbodyMotor;
            rigidbody = rbMotor.MyRigidbody;

            if(rbMotor == null ) 
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

        public override void Update()
        {
            curMovementDir = Vector3.Lerp(curMovementDir, curInput, movementCatchUpSpeed * Time.deltaTime);
            curLookDir = motor.transform.forward;

            float totalAngleDiff = Vector3.SignedAngle(curLookDir, curInput, Vector3.up);
            float curRotationSpeed = anuglarVelocityUpSpeed * Mathf.Sign(totalAngleDiff) * Time.deltaTime;

            if((curRotationSpeed > 0 && totalAngleDiff < curRotationSpeed) || (curRotationSpeed < 0 && totalAngleDiff > curRotationSpeed))
                curRotationSpeed = totalAngleDiff;

            Quaternion rot = Quaternion.AngleAxis(curRotationSpeed, Vector3.up);

            curLookDir = rot * curLookDir;
            curLookDir *= curMovementDir.magnitude;

            rbMotor.InputMotionVector = curLookDir;
            rbMotor.RawInputMotionVector = curLookDir;

            //If we're not receiving input, then don't change look direction.
            if(curInput.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(curInput);

                Quaternion rotation = rigidbody.rotation;
                rotation = Quaternion.Slerp(rotation, targetRotation, rotationCatchUpSpeed * Time.deltaTime);

                rigidbody.MoveRotation(rotation);
            }
        }
        private void OnInputReceived(Vector2 input)
        {
            curInput.x = input.x;
            curInput.z = input.y;
        }
    }
}