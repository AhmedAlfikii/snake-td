using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/Motor/Basic Dash"), GraphNodeName("Basic Dash")]
    public class BasicDashNode : AbilityTaskNode
    {
        [SerializeField] private BlackboardAdvancedFloatGetter dashLength = new BlackboardAdvancedFloatGetter(5f);
        [SerializeField] private BlackboardAdvancedFloatGetter dashSpeed = new BlackboardAdvancedFloatGetter(5f);
        [SerializeField] private ScriptableVector3 inputVector;

        [NonSerialized] private float startTime;
        [NonSerialized] private Vector3 startPosition;
        [NonSerialized] private Vector3 targetPosition;
        [NonSerialized] private Rigidbody characterRigidbody;
        [NonSerialized] private float dashDuration = 0.25f;
        [NonSerialized] private float speed;
        [NonSerialized] private Vector3 dir;

        public BasicDashNode(BasicDashNode other) : base(other)
        {
            dashLength = new BlackboardAdvancedFloatGetter(other.dashLength);
            dashSpeed = new BlackboardAdvancedFloatGetter(other.dashSpeed);
            inputVector = other.inputVector;
            characterRigidbody = other.characterRigidbody;
        }
        public BasicDashNode()
        {

        }

        protected override void OnInitialize()
        {
            characterRigidbody = actor.GetCachedComponent<Rigidbody>();
            dashLength.Initialize(ability.BlackboardCollection);
            dashSpeed.Initialize(ability.BlackboardCollection);
        }
        protected override void OnStart()
        {
            startTime = Time.time;
            startPosition = characterRigidbody.position;

            Vector3 input = inputVector != null ? inputVector.Value : Vector3.zero;

            if (input.sqrMagnitude > 0.001f)
                dir = input.normalized;
            else
                dir = characterRigidbody.transform.forward;

            speed = dashSpeed.Value.Value;
            dashDuration = dashLength.Value.Value / speed;

            characterRigidbody.transform.forward = dir;
        }
        protected override BTNodeState OnUpdate()
        {
            if (characterRigidbody == null)
                return BTNodeState.Success;

            if(Time.time < startTime + dashDuration)
            {
                characterRigidbody.linearVelocity = dir * speed;

                return BTNodeState.Running;
            }
            
            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            BasicDashNode clonedNode = new BasicDashNode(this);

            return clonedNode;
        }
    }
}