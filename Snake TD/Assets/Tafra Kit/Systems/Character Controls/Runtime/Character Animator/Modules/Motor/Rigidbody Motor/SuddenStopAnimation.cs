using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Motor/Rigidbody/Sudden Stop Animation")]
    public class SuddenStopAnimation : CharacterAnimatorModule
    {
        [SerializeField] private float deltaThreshold = 1f;
        [SerializeField] private float minimumMovementDuration = 0.5f;

        [Header("State")]
        [SerializeField] private string suddenStopStateName = "Run End";
        [SerializeField] private int layer = 0;
        [SerializeField] private float normalizedTransitionDuration = 0.1f;
        [SerializeField] private float normalizedTimeOffset = 0f;

        private Rigidbody rigidbody;
        private float deltaThresholdSqred;
        private int suddenStopStateNameHash;
        private Vector3 prevVelocity;
        private float lastStationaryTime;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => true;

        protected override void OnInitialize()
        {
            rigidbody = actor.GetCachedComponent<Rigidbody>();
            deltaThresholdSqred = deltaThreshold * deltaThreshold;
            suddenStopStateNameHash = Animator.StringToHash(suddenStopStateName);
        }
        public override void OnControllerStart()
        {
            prevVelocity = rigidbody.linearVelocity;
        }
        public override void FixedUpdate()
        {
            Vector3 velocity = rigidbody.linearVelocity;

            Vector3 velocityDelta = velocity - prevVelocity;

            //Stopped while moving.
            if(velocity.sqrMagnitude < 0.01f)
            {
                if(velocityDelta.sqrMagnitude > deltaThresholdSqred && Time.time > lastStationaryTime + minimumMovementDuration)
                    animator.CrossFade(suddenStopStateNameHash, normalizedTransitionDuration, layer, normalizedTimeOffset);

                lastStationaryTime = Time.time;
            }

            prevVelocity = velocity;
        }
    }
}