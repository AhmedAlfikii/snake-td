using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Motor
{
    public class BodyLookAtTargetAction : MotorAction
    {
        protected Transform targetTransform;
        protected Vector3 targetPoint;
        protected Transform motorTransform;
        protected float lookSpeed;
        protected bool completeOnArrival;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        public BodyLookAtTargetAction(AIMotor motor) : base(motor)
        {
            motorTransform = motor.transform;
        }

        public void Initialize(Transform target, float lookSpeed, bool completeOnArrival)
        {
            this.targetTransform = target;
            this.lookSpeed = lookSpeed;
            this.completeOnArrival = completeOnArrival;
        }
        public void Initialize(Vector3 targetPoint, float lookSpeed, bool completeOnArrival)
        {
            this.targetPoint = targetPoint;
            this.lookSpeed = lookSpeed;
            this.completeOnArrival = completeOnArrival;

            targetTransform = null;
        }

        public override void OnUpdate()
        {
            Vector3 targetPos = targetTransform != null ? targetTransform.transform.position : targetPoint;
            targetPos.y = motorTransform.position.y;

            float lookSpeedValue = lookSpeed;

            Vector3 dir = targetPos - motorTransform.position;
            
            if(lookSpeedValue > 0)
            {
                motorTransform.rotation = Quaternion.Slerp(motorTransform.rotation, Quaternion.LookRotation(dir), lookSpeedValue * Time.deltaTime);

                if(completeOnArrival && Vector3.Dot(dir.normalized, motorTransform.forward) > 0.99f)
                {
                    //Just to make sure they are perfectly aligned before completing since we consider it aligned at 0.99.
                    motorTransform.rotation = Quaternion.LookRotation(dir);
                    Complete();
                }
            }
            else
            {
                motorTransform.rotation = Quaternion.LookRotation(dir);

                if(completeOnArrival)
                    Complete();
            }
        }
    }
}