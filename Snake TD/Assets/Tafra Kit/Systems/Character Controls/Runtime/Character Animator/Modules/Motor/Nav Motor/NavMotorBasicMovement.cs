using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Motor/Navigation Agent/Basic Movement")]
    public class NavMotorBasicMovement : CharacterAnimatorModule
    {
        [Header("Animator Properties")]
        [Tooltip("The name of the bool animator parameter that should be toggled on as long as the agent is moving.")]
        [SerializeField] private string isMovingParam = "Is Moving";

        [Header("Over-speed")]
        [Tooltip("If enabled, the movement animation speed will increase if the agent's speed exceeded the normal speed. Up the value of the max animation speed field.")]
        [SerializeField] private bool enableOverSpeed;
        [Tooltip("The movement speed of the agent that is ideal for the movement animation (would not make the animation feel \"slidey\". " +
        "The blend-tree axes will reach 1 when the agent's speed reaches this value.")]
        [SerializeField] private float normalSpeed = 1;
        [Tooltip("The name of the float animator parameter that controls the speed of the blend tree")]
        [SerializeField] private string animationSpeedParam = "Movement Speed Multiplier";
        [Tooltip("If the agent's speed exceeded the normal speed, its movement animation speed will increase based on the percentage of extra speed, up to this value.")]
        [SerializeField] private float maxAnimationSpeed = 1.5f;


        private NavMeshAgent navAgent;
        private Transform agentTransform;
        private int isMovingParamHash;
        private int animationSpeedParamHash;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => true;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            navAgent = actor.GetCachedComponent<NavMeshAgent>();
            agentTransform = navAgent.transform;

            isMovingParamHash = Animator.StringToHash(isMovingParam);

            if (enableOverSpeed)
                animationSpeedParamHash = Animator.StringToHash(animationSpeedParam);
        }

        public override void LateUpdate()
        {
            bool isMoving = navAgent.velocity.sqrMagnitude > 0.01f;

            animator.SetBool(isMovingParamHash, isMoving);

            if(isMoving && enableOverSpeed)
            {
                float cappedMultiplier = Mathf.Min(maxAnimationSpeed, Mathf.Abs(navAgent.velocity.magnitude / normalSpeed));

                //Animation speed should not drop below 0.5f (arbitrary value, no scientific purpose behind it).
                float animationSpeedMultiplier = Mathf.Max(0.5f, cappedMultiplier);

                animator.SetFloat(animationSpeedParamHash, animationSpeedMultiplier);
            }
        }
    }
}