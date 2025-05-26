using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.InputSystem;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Rigidbody/Body Rotation (RB)")]
    public class RBBodyRotationModule : BodyRotationModule
    {
        [SerializeField] private LookState defaultLookState = LookState.LookAtMovementDirection;

        private RigidbodyMotor rbMotor;
        private Rigidbody rigidbody;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => true;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            rbMotor = motor as RigidbodyMotor;
            rigidbody = rbMotor.MyRigidbody;
            lookState = defaultLookState;

            if(rbMotor == null ) 
                TafraDebugger.Log("Movement With Player Input Module", "\"Movement With Player Input Module\" only works with RigidbodyMotors.", TafraDebugger.LogType.Error);
        }

        public override void FixedUpdate()
        {
            Quaternion rotation = rigidbody.rotation;

            switch(lookState)
            {
                case LookState.LookAtMovementDirection:
                    {
                        Vector3 movementDir = rigidbody.linearVelocity;
                        movementDir.y = 0;

                        //If the rigidbody is currently not moving, then don't change look direction.
                        if(movementDir.sqrMagnitude < 0.01f)
                            return;

                        Quaternion targetRotation = Quaternion.LookRotation(movementDir);

                        rotation = Quaternion.Slerp(rotation, targetRotation, movementDirectionLookSpeed * Time.deltaTime);

                        rigidbody.MoveRotation(rotation);
                        break;
                    }
                case LookState.LookAtMovementInput:
                    {
                        Vector3 movementDir = rbMotor.InputMotionVector;
                        movementDir.y = 0;

                        //If the rigidbody is currently not moving, then don't change look direction.
                        if(movementDir.sqrMagnitude < 0.01f)
                            return;

                        Quaternion targetRotation = Quaternion.LookRotation(movementDir);

                        rotation = Quaternion.Slerp(rotation, targetRotation, movementDirectionLookSpeed * Time.deltaTime);

                        rigidbody.MoveRotation(rotation);
                        break;
                    }

                case LookState.LookAtTargetDirection:
                    {
                        Vector3 dir = targetLookDirection;
                        dir.y = 0;

                        Quaternion targetRotation = Quaternion.LookRotation(dir);

                        rotation = Quaternion.Slerp(rotation, targetRotation, movementDirectionLookSpeed * Time.deltaTime);

                        rigidbody.MoveRotation(rotation);
                        break;
                    }

                case LookState.LookAtLockedTarget:
                    {
                        if (lockedLookAtTarget == null) 
                            return;

                        Vector3 dir = lockedLookAtTarget.position - rigidbody.position;
                        dir.y = 0;

                        Quaternion targetRotation = Quaternion.LookRotation(dir);

                        rotation = Quaternion.Slerp(rotation, targetRotation, movementDirectionLookSpeed * Time.deltaTime);

                        rigidbody.MoveRotation(rotation);
                        break;
                    }
            }
        }
    }
}