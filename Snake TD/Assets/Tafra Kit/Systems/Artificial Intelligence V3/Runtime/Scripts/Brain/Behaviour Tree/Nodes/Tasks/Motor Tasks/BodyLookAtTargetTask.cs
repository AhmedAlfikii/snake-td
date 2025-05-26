using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3.Motor;
using TafraKit.AI3;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Motor/Body Look At Target"), GraphNodeName("Body Look At Target", "Body Look At")]
    public class BodyLookAtTargetTask : MotorActionTask
    {
        public enum TargetType
        { 
            Transform,
            Point
        }
        [Tooltip("Transform: lock on the target transform and keep looking at it even if it moves. Point: cache the target point on start, and keep looking at it.")]
        [SerializeField] private TargetType targetType;
        [SerializeField] private BlackboardActorGetter target;
        [SerializeField] private BlackboardDynamicPointGetter targetPoint;
        [Tooltip("The speed of which the motor will look at it's target, use this to smooth the rotation. 0 or less means no smoothing.")]
        [SerializeField] private BlackboardFloatGetter lookSpeed;
        [SerializeField] private bool completeOnAlign;

        protected override AIMotor.ActionCategories ActionCategory => AIMotor.ActionCategories.MainRotation;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            target.Initialize(agent.BlackboardCollection);
            targetPoint.Initialize(agent.BlackboardCollection);
            lookSpeed.Initialize(agent.BlackboardCollection);
        }

        protected override int StartAction()
        {
            TafraActor targetActor = target.Value;

            if (targetActor != null)
                return motor.BodyLookAtTarget(targetActor.transform, lookSpeed.Value, completeOnAlign, OnActionCompleted, OnActionInterrupted);
            else
                return motor.BodyLookAtTarget(targetPoint.Value, lookSpeed.Value, completeOnAlign, OnActionCompleted, OnActionInterrupted);
        }
    }
}