using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Motor/Rigidbody/Two Dimensional Movement (RB)")]
    public class RigidMotor2DBlendMovement : Motor2DBlendMovement
    {
        private Rigidbody rigidbody;

        protected override void OnInitialize()
        {
            rigidbody = actor.GetCachedComponent<Rigidbody>();
            motorTransform = rigidbody.transform;

            base.OnInitialize();
        }

        public override void LateUpdate()
        {
            localVelocity = motorTransform.InverseTransformVector(rigidbody.linearVelocity);

            base.LateUpdate();
        }
    }
}