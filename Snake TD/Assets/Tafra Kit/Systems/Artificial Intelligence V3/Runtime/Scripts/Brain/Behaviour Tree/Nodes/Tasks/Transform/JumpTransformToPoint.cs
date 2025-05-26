using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using UnityEngine.AI;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Transform/Jump Transform to Point"), GraphNodeName("Jump Transform to Point")]
    public class JumpTransformToPoint : TaskNode
    {
        [SerializeField] private bool agentTransformIsTarget;
        [SerializeField] private BlackboardGameObjectGetter targetTransform;
        [SerializeField] private BlackboardDynamicPointGetter targetPoint;
        [SerializeField] private BlackboardDynamicFloatGetter travelDuration;
        [SerializeField] private BlackboardDynamicFloatGetter jumpHeight;

        private Transform targetTransformValue;
        private Vector3 startPosition;
        private Vector3 midPosition;
        private float startTime;
        private float duration;
        private float endTime;

        protected override void OnInitialize()
        {
            targetTransform.Initialize(agent.BlackboardCollection);
            targetPoint.Initialize(agent.BlackboardCollection);
            travelDuration.Initialize(agent.BlackboardCollection);
            jumpHeight.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            targetTransformValue = agentTransformIsTarget ? agent.transform : targetTransform.Value.transform;
            duration = travelDuration.Value;

            startPosition = targetTransformValue.position;
            
            Vector3 endPosition = targetPoint.Value;

            midPosition = startPosition + (endPosition - startPosition) / 2f + new Vector3(0, jumpHeight.Value, 0);

            startTime = Time.time;
            endTime = startTime + duration;
        }
        protected override BTNodeState OnUpdate()
        {
            if (Time.time < endTime)
            { 
                float t = (Time.time - startTime) / duration;

                targetTransformValue.position = ZBezier.GetPointOnQuadraticCurve(t, startPosition, midPosition, targetPoint.Value);

                return BTNodeState.Running;
            }

            targetTransformValue.position = targetPoint.Value;

            return BTNodeState.Success;
        }
    }
}