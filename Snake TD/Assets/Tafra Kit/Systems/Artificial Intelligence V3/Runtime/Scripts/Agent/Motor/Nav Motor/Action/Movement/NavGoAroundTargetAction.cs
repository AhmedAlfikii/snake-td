using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.AI3.Motor
{
    public class NavGoAroundTargetAction : GoAroundTargetAction
    {
        private AINavMotor navMotor;
        private NavMeshAgent navAgent;
        private int startFrame;

        private const string agentSpeedInfluenceID = "navGoAroundTargetAction";

        public NavGoAroundTargetAction(AIMotor motor) : base(motor)
        {
            navMotor = motor as AINavMotor;
            navAgent = navMotor.NavAgent;
            motorTransform = motor.transform;
        }

        public override void OnStart()
        {
            base.OnStart();

            if (navAgent.enabled)
                navAgent.SetDestination(destination);

            if(speed > 0)
                navMotor.SetAgentSpeed(agentSpeedInfluenceID, speed * motor.MovementSpeedMultiplier);

            navMotor.MovementSpeedMultiplierChange.AddListener(OnMovementSpeedMultiplierChange);
           
            startFrame = Time.frameCount;
        }
        public override void OnUpdate()
        {
            //Avoid processing on the same frame the destinatino was set in to give the agent a chance to notice that a path should be calcualted.
            if(Time.frameCount == startFrame)
                return;

            if(!navAgent.enabled)
                return;

            if(navAgent.pathPending == true)
                return;

            if(navAgent.remainingDistance <= navAgent.stoppingDistance)
                Complete();
        }
        public override void OnConclude()
        {
            if(speed > 0)
                navMotor.ResetAgentSpeed(agentSpeedInfluenceID);

            if (navAgent.enabled)
                navAgent.ResetPath();

            navMotor.MovementSpeedMultiplierChange.RemoveListener(OnMovementSpeedMultiplierChange);
        }

        private void OnMovementSpeedMultiplierChange(float multiplier)
        {
            if(speed > 0)
                navMotor.SetAgentSpeed(agentSpeedInfluenceID, speed * motor.MovementSpeedMultiplier);
        }
    }
}
