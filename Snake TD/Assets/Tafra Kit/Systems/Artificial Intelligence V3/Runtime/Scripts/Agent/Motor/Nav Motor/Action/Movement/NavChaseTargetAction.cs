using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TafraKit;

namespace TafraKit.AI3.Motor
{
    public class NavChaseTargetAction : ChaseTargetAction
    {
        private AINavMotor navMotor;
        private NavMeshAgent navAgent;
        private NavMeshPath path = new NavMeshPath();
        private float sqrStoppingDistance;

        private const string agentSpeedInfluenceID = "navChaseTargetAction";

        public NavChaseTargetAction(AIMotor motor) : base(motor)
        {
            navMotor = motor as AINavMotor;
            navAgent = navMotor.NavAgent;
        }
        public override void OnStart()
        {
            if (chaseSpeed > 0)
                navMotor.SetAgentSpeed(agentSpeedInfluenceID, chaseSpeed * motor.MovementSpeedMultiplier);

            sqrStoppingDistance = stoppingDistance * stoppingDistance;

            navMotor.MovementSpeedMultiplierChange.AddListener(OnMovementSpeedMultiplierChange);
        }
        public override void OnUpdate()
        {
            path.ClearCorners();

            if(navAgent.enabled)
            {
                bool foundPath = navAgent.CalculatePath(target.position, path);

                if(!foundPath)
                    return;
            }
            else
                return;

            if(completeOnArrival && GetPathSqrLength(path) <= sqrStoppingDistance)
            {
                Complete();
                return;
            }

            navAgent.SetPath(path);
        }
        public override void OnConclude()
        {
            if(chaseSpeed > 0)
                navMotor.ResetAgentSpeed(agentSpeedInfluenceID);

            if (navAgent.enabled)
                navAgent.ResetPath();

            navMotor.MovementSpeedMultiplierChange.RemoveListener(OnMovementSpeedMultiplierChange);
        }

        private float GetPathSqrLength(NavMeshPath path)
        {
            float length = 0;

            for(int i = 1; i < path.corners.Length; i++)
            {
                length += (path.corners[i - 1] - path.corners[i]).sqrMagnitude;
            }

            return length;
        }
        private void OnMovementSpeedMultiplierChange(float multiplier)
        {
            if (chaseSpeed > 0)
                navMotor.SetAgentSpeed(agentSpeedInfluenceID, chaseSpeed * motor.MovementSpeedMultiplier);
        }
    }
}