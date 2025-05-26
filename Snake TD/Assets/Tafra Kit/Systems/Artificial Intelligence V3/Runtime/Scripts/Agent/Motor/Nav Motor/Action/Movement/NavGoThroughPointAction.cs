using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.AI3.Motor
{
    public class NavGoThroughPointAction : GoThroughPointAction
    {
        private AINavMotor navMotor;
        private NavMeshAgent navAgent;
        private Vector3 destination;

        //Dash related
        private float dashDuration;
        private Vector3 startPosition;
        private float startTime;
        private float endTime;
      
        private const string agentSpeedInfluenceID = "navGoThroughPointAction";

        public NavGoThroughPointAction(AIMotor motor) : base(motor)
        {
            navMotor = motor as AINavMotor;
            navAgent = navMotor.NavAgent;
        }

        public override void OnStart()
        {
            Vector3 dirToTarget = targetPoint - motorTransform.position;
            Vector3 dirToTargetNormalized = dirToTarget.normalized;

            if(motionType == GoThroughTargetType.Dash && !canDashThroughObstacles)
            {
                #region Detect if there are obstacles between motor and target
                Vector3 startPoint = motorTransform.position + new Vector3(0, 0.25f, 0);
                Vector3 endPoint = targetPoint + new Vector3(0, 0.25f, 0);

                bool foundObstacle = Physics.Linecast(startPoint, endPoint, obstaclesLayerMask, QueryTriggerInteraction.Ignore);

                if(foundObstacle)
                {
                    Complete();
                    return;
                }
                #endregion

                //Find the furtherest point beyond the target before colliding with an obstacle.
                if (Physics.Raycast(endPoint, dirToTargetNormalized, out var rayHit, distanceAfterTarget, obstaclesLayerMask, QueryTriggerInteraction.Ignore))
                    distanceAfterTarget = rayHit.distance;
            }

            dirToTarget += dirToTargetNormalized * distanceAfterTarget;
            destination = motorTransform.position + dirToTarget;

            NavMeshHit hit = default;
            int iterations = 0;
            bool foundPoint = false;

            while(!foundPoint)
            {
                foundPoint = NavMesh.SamplePosition(destination, out hit, 5, 1);

                iterations++;
                if(iterations >= 5)
                    break;
            }

            if(foundPoint)
                destination = hit.position;

            if(motionType == GoThroughTargetType.Dash)
            {
                navAgent.enabled = false;
                startPosition = motorTransform.position;
                startTime = Time.time;
                dashDuration = (destination - startPosition).magnitude / speed;
                endTime = startTime + dashDuration;
            }
            else
            {
                if(speed > 0)
                    navMotor.SetAgentSpeed(agentSpeedInfluenceID, speed);

                navAgent.SetDestination(destination);
            }
        }
        public override void OnUpdate()
        {
            if(motionType == GoThroughTargetType.Dash)
            {
                if(Time.time < endTime)
                {
                    float t = (Time.time - startTime) / dashDuration;

                    motorTransform.position = Vector3.Lerp(startPosition, destination, t);
                }
                else
                {
                    motorTransform.position = destination;

                    Complete();
                }
            }
            else
            {
                if(!navAgent.enabled)
                    return;

                if(navAgent.pathPending == true)
                    return;

                if(!navAgent.hasPath)
                {
                    Complete();
                    return;
                }

                if(navAgent.remainingDistance <= navAgent.stoppingDistance)
                    Complete();
            }
        }
        public override void OnConclude()
        {
            if(motionType == GoThroughTargetType.Dash)
                navAgent.enabled = true;
            else
            {
                if(speed > 0)
                    navMotor.ResetAgentSpeed(agentSpeedInfluenceID);

                if (navAgent.enabled)
                    navAgent.ResetPath();
            }
        }
    }
}