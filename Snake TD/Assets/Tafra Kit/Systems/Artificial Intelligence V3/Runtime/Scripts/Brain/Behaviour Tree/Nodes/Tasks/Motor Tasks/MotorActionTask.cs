using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.AI3.Motor;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    public abstract class MotorActionTask : TaskNode
    {
        protected AIMotor motor;
        protected int actionID;
        protected bool isActing;

        protected abstract AIMotor.ActionCategories ActionCategory { get; }

        protected override void OnInitialize()
        {
            motor = agent.GetCachedComponent<AIMotor>();
        }
        protected override void OnStart()
        {
            actionID = StartAction();

            isActing = actionID != -1;
        }
        protected override BTNodeState OnUpdate()
        {
            if(isActing)
                return BTNodeState.Running;
            else
                return BTNodeState.Success;
        }
        protected override void OnReset()
        {
            if(isActing)
            {
                motor.StopAction(actionID, ActionCategory);
                isActing = false;
            }
        }
        protected void OnActionCompleted()
        {
            isActing = false;
        }
        protected void OnActionInterrupted()
        {
            isActing = false;
        }

        protected abstract int StartAction();
    }
}