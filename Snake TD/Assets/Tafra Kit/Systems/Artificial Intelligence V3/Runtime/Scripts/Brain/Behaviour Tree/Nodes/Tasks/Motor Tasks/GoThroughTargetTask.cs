using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.AI3.Motor;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Motor/Go Through Target"), GraphNodeName("Go Through Target", "Through Target")]
    public class GoThroughTargetTask : MotorActionTask
    {
        [Tooltip("If dash, the motor will dash through the object not worrying about colliding with it. Ex: NavMeshAgent will be disabled during dashes.")]
        [SerializeField] private GoThroughPointAction.GoThroughTargetType motionType;
        [SerializeField] private BlackboardActorGetter target;
        [SerializeField] private BlackboardFloatGetter distanceAfterTarget;
        [SerializeField] private BlackboardFloatGetter speed = new BlackboardFloatGetter(10f);
        [SerializeField] private BlackboardBoolGetter canDashThroughObstacles;
        [SerializeField] private LayerMask obstaclesLayerMask = 1;

        protected override AIMotor.ActionCategories ActionCategory => AIMotor.ActionCategories.MainMovement;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            target.Initialize(agent.BlackboardCollection);
            distanceAfterTarget.Initialize(agent.BlackboardCollection);
            speed.Initialize(agent.BlackboardCollection);
        }

        protected override int StartAction()
        {
            return motor.GoThroughPoint(target.Value.transform.position, motionType, distanceAfterTarget.Value, speed.Value, canDashThroughObstacles.Value, obstaclesLayerMask, OnActionCompleted, OnActionInterrupted);
        }
    }
}