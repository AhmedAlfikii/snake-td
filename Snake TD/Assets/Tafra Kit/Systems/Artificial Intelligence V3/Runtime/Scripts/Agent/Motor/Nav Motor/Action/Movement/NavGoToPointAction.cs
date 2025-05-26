using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.AI3.Motor
{
    public class NavGoToPointAction : GoToPointAction
    {
        private AINavMotor navMotor;
        private NavMeshAgent navAgent;

        private int startFrame;

        public NavGoToPointAction(AIMotor motor) : base(motor)
        {
            navMotor = motor as AINavMotor;
            navAgent = navMotor.NavAgent;
        }
        
        public override void OnStart()
        {
            startFrame = Time.frameCount;
            navAgent.SetDestination(destination);
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

            if(!navAgent.hasPath)
            {
                Complete();
                return;
            }

            if(navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                Complete();
            }
        }
        public override void OnConclude()
        {
            if(navAgent.enabled)
                navAgent.ResetPath();
        }
    }
}