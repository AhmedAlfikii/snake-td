using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.AI3.Motor;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Motor/Go Through Point"), GraphNodeName("Go Through Point", "Through Point")]
    public class GoThroughPointTask : MotorActionTask
    {
        [Tooltip("If dash, the motor will dash through the object not worrying about colliding with it. Ex: NavMeshAgent will be disabled during dashes.")]
        [SerializeField] private GoThroughPointAction.GoThroughTargetType motionType;
        [SerializeField] private BlackboardVector3Getter point;
        [SerializeField] private BlackboardDynamicFloatGetter distanceAfterTarget;
        [SerializeField] private BlackboardDynamicFloatGetter speed = new BlackboardDynamicFloatGetter(10f);
        [SerializeField] private BlackboardBoolGetter canDashThroughObstacles;
        [SerializeField] private TafraLayerMask obstaclesLayerMask = new TafraLayerMask(1, null);

        protected override AIMotor.ActionCategories ActionCategory => AIMotor.ActionCategories.MainMovement;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            point.Initialize(agent.BlackboardCollection);
            distanceAfterTarget.Initialize(agent.BlackboardCollection);
            speed.Initialize(agent.BlackboardCollection);
        }

        protected override int StartAction()
        {
            return motor.GoThroughPoint(point.Value, motionType, distanceAfterTarget.Value, speed.Value, canDashThroughObstacles.Value, obstaclesLayerMask.Value, OnActionCompleted, OnActionInterrupted);
        }
    }
}