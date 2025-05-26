using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Motor/Navigation Agent/Two Dimensional Movement (Nav)")]
    public class NavMotor2DBlendMovement : Motor2DBlendMovement
    {
        private NavMeshAgent navAgent;

        protected override void OnInitialize()
        {
            navAgent = actor.GetCachedComponent<NavMeshAgent>();
            motorTransform = navAgent.transform;

            base.OnInitialize();
        }

        public override void LateUpdate()
        {
            localVelocity = motorTransform.InverseTransformVector(navAgent.velocity);

            base.LateUpdate();
        }
    }
}