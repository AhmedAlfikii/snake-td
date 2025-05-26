using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Motor
{
    public class ChaseTargetAction : MotorAction
    {
        protected Transform target;
        protected Transform motorTransform;
        protected float chaseSpeed;
        protected float stoppingDistance;
        protected bool completeOnArrival;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        public ChaseTargetAction(AIMotor motor) : base(motor)
        {
            motorTransform = motor.transform;
        }
        public void Initialize(Transform target, float chaseSpeed, float stoppingDistance, bool completeOnArrival)
        {
            this.target = target;
            this.chaseSpeed = chaseSpeed;
            this.stoppingDistance = stoppingDistance;
            this.completeOnArrival = completeOnArrival;
        }

        public override void OnUpdate()
        {
            motorTransform.position = Vector3.MoveTowards(motorTransform.position, target.position, chaseSpeed * Time.deltaTime);

            if(completeOnArrival)
            {
                Vector3 dir = target.position - motorTransform.position;
                if(dir.magnitude <= stoppingDistance)
                    Complete();
            }
        }
    }
}