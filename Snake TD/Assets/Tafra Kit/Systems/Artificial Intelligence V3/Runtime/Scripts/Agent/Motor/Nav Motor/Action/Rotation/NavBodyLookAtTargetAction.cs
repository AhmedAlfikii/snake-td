using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.AI3.Motor
{
    public class NavBodyLookAtTargetAction : BodyLookAtTargetAction
    {
        private AINavMotor navMotor;
        private NavMeshAgent navAgent;
        private bool originalUpdateRotation;

        public NavBodyLookAtTargetAction(AIMotor motor) : base(motor)
        {
            navMotor = motor as AINavMotor;
            navAgent = navMotor.NavAgent;
        }

        public override void OnStart()
        {
            originalUpdateRotation = navAgent.updateRotation;

            navAgent.updateRotation = false;
        }
        public override void OnComplete()
        {
            navAgent.updateRotation = originalUpdateRotation;
        }
        public override void OnInterrupt()
        {
            navAgent.updateRotation = originalUpdateRotation;
        }
    }
}