using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Motor
{
    public class GoToPointAction : MotorAction
    {
        protected Vector3 destination;
        protected Transform motorTransform;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        public GoToPointAction(AIMotor motor) : base(motor)
        {
            motorTransform = motor.transform;
        }
        public void Initialize(Vector3 destination)
        {
            this.destination = destination;
        }

        public override void OnUpdate()
        {
            motorTransform.position = Vector3.MoveTowards(motorTransform.position, destination, 4 * Time.deltaTime);

            Vector3 dir = destination - motorTransform.position;
            if(dir.sqrMagnitude < 0.5f)
                Complete();
        }
    }
}
