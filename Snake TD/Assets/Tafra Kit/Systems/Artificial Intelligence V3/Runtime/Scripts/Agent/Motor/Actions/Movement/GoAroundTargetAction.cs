using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Motor
{
    public class GoAroundTargetAction : MotorAction
    {
        protected Vector3 targetPoint;
        protected float speed;
        protected bool curDistanceIsOrbitDistance;
        protected float orbitDistance;
        protected float angle;
        protected Transform motorTransform;
        protected Vector3 destination;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        public GoAroundTargetAction(AIMotor motor) : base(motor)
        {
            motorTransform = motor.transform;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPoint">The point to go around.</param>
        /// <param name="speed">The movement speed.</param>
        /// <param name="curDistanceIsOrbitDistance">Use the current distance between the motor and the target point as the orbit distance.</param>
        /// <param name="orbitDistance">Only used if curDistanceIsOrbitDistance is set to false. The distance between the target and the point around it (circle radius).</param>
        /// <param name="angle">The angle difference between the current angle from motor to target, and the final point to target angle (ex. move 10 or -10 degrees around target)</param>
        public void Initialize(Vector3 targetPoint, float speed, bool curDistanceIsOrbitDistance, float orbitDistance, float angle)
        {
            this.targetPoint = targetPoint;
            this.speed = speed;
            this.curDistanceIsOrbitDistance = curDistanceIsOrbitDistance;
            this.orbitDistance = orbitDistance;
            this.angle = angle;
        }
        public override void OnStart()
        {
            Vector3 targetLeveledPosition = targetPoint;
            targetLeveledPosition.y = motorTransform.position.y;

            Vector3 targetToMe = (motorTransform.position - targetLeveledPosition);
            targetToMe.Normalize();

            //Do angle manipulation here.

            Vector3 rotatedDir = (Quaternion.AngleAxis(angle, Vector3.up)) * targetToMe;

            float radius = curDistanceIsOrbitDistance ? Vector3.Distance(motorTransform.position, targetLeveledPosition) : orbitDistance;

            destination = targetPoint + rotatedDir * radius;
        }
        public override void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}
