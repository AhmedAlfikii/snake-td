using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3.Motor;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Motor/Chase Target"), GraphNodeName("Chase Target", "Chase")]
    public class ChaseTargetTask : MotorActionTask
    {
        [SerializeField] private BlackboardActorGetter target;
        [SerializeField] private BlackboardDynamicFloatGetter chaseSpeed = new BlackboardDynamicFloatGetter(4);
        [SerializeField] private BlackboardDynamicFloatGetter stoppingDistance = new BlackboardDynamicFloatGetter(0);
        [SerializeField] private bool completeOnArrival = true;

        protected override AIMotor.ActionCategories ActionCategory => AIMotor.ActionCategories.MainMovement;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            target.Initialize(agent.BlackboardCollection);
            chaseSpeed.Initialize(agent.BlackboardCollection);
            stoppingDistance.Initialize(agent.BlackboardCollection);
        }

        protected override int StartAction()
        {
            return motor.ChaseTarget(target.Value.transform, chaseSpeed.Value, stoppingDistance.Value, completeOnArrival, completeOnArrival ? OnActionCompleted : null, OnActionInterrupted);
        }
    }
}