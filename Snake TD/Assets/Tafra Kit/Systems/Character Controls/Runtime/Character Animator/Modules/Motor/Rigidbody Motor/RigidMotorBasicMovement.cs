using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Motor/Rigidbody/Basic Movement (RB)")]
    public class RigidMotorBasicMovement : CharacterAnimatorModule
    {
        [Header("Animator Properties")]
        [Tooltip("The name of the float animator parameter that should receive the movement speed.")]
        [SerializeField] protected string movementSpeedParam = "Movement Speed";

        private Rigidbody rigidbody;
        private Transform motorTransform;
        private int movementSpeedParamHash;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => true;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            rigidbody = actor.GetCachedComponent<Rigidbody>();
            motorTransform = rigidbody.transform;
            movementSpeedParamHash = Animator.StringToHash(movementSpeedParam);

            base.OnInitialize();
        }

        public override void LateUpdate()
        {
            float movementSpeed = rigidbody.linearVelocity.magnitude;
            animator.SetFloat(movementSpeedParamHash, movementSpeed);
        }
    }
}