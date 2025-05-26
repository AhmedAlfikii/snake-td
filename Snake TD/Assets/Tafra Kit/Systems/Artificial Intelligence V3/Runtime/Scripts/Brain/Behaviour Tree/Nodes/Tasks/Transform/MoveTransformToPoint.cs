using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using UnityEngine.AI;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Transform/Move Transform to Point"), GraphNodeName("Move Transform to Point")]
    public class MoveTransformToPoint : TaskNode
    {
        [SerializeField] private bool agentTransformIsTarget;
        [SerializeField] private BlackboardGameObjectGetter targetTransform;
        [SerializeField] private BlackboardDynamicPointGetter targetPoint;
        [SerializeField] private BlackboardDynamicFloatGetter travelDuration;

        private Transform targetTransformValue;
        private Vector3 startPosition;
        private float startTime;
        private float duration;
        private float endTime;

        protected override void OnInitialize()
        {
            targetTransform.Initialize(agent.BlackboardCollection);
            targetPoint.Initialize(agent.BlackboardCollection);
            travelDuration.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            targetTransformValue = agentTransformIsTarget ? agent.transform : targetTransform.Value.transform;
            duration = travelDuration.Value;

            startPosition = targetTransformValue.position;
            startTime = Time.time;
            endTime = startTime + duration;
        }
        protected override BTNodeState OnUpdate()
        {
            if (Time.time < endTime)
            { 
                float t = (Time.time - startTime) / duration;

                targetTransformValue.position = Vector3.LerpUnclamped(startPosition, targetPoint.Value, t);

                return BTNodeState.Running;
            }

            targetTransformValue.position = targetPoint.Value;

            return BTNodeState.Success;
        }
    }
}