using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Motor
{
    public class GoThroughPointAction : MotorAction
    {
        public enum GoThroughTargetType
        {
            Dash,
            WalkOrRun
        }

        protected Transform motorTransform;
        protected Vector3 targetPoint;
        protected GoThroughTargetType motionType;
        protected float distanceAfterTarget;
        protected float speed;
        protected bool canDashThroughObstacles;
        protected LayerMask obstaclesLayerMask;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        public GoThroughPointAction(AIMotor motor) : base(motor)
        {
            motorTransform = motor.transform;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPoint">The position to go through.</param>
        /// <param name="motionType">If dash, the motor will dash through the object not worrying about colliding with it. Ex: NavMeshAgent will be disabled during dashes.</param>
        /// <param name="distanceAfterTarget">The distance to move beyond the target.</param>
        /// <param name="speed">The speed of which the character will dash if it's enabled.</param>
        /// <param name="canDashThroughObstacles">If motion type is dashing, should the motor be able to dash through obstacles (colliders)? If not, this action will complete without dashing if an obstacle is in the way.</param>
        /// <param name="obstaclesLayerMask">The layer mask of the dashing obstacles.</param>
        public void Initialize(Vector3 targetPoint, GoThroughTargetType motionType, float distanceAfterTarget, float speed, bool canDashThroughObstacles, LayerMask obstaclesLayerMask)
        {
            this.targetPoint = targetPoint;
            this.motionType = motionType;
            this.distanceAfterTarget = distanceAfterTarget;
            this.speed = speed;
            this.canDashThroughObstacles = canDashThroughObstacles;
            this.obstaclesLayerMask = obstaclesLayerMask;
        }
        public override void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}
