using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.GraphViews
{
    [SearchMenuItem("Positioning/Distance To Actor")]
    public class BlackboardDistanceToActorCondition : BlackboardCondition
    {
        [SerializeField] private BlackboardActorGetter target;
        [SerializeField] private BlackboardDynamicFloatGetter targetDistance;
        [SerializeField] private NumberRelation relation = NumberRelation.GreaterThan;

        private Transform agentTransform;
        private float sqrDistance;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            target.Initialize(blackboardCollection);
            targetDistance.Initialize(blackboardCollection);

            agentTransform = actor.transform;

            float distanceValue = targetDistance.Value;
            sqrDistance = distanceValue * distanceValue;

        }
        protected override bool PerformCheck()
        {
            TafraActor value = target.Value;

            if(value == null)
            {
                TafraDebugger.Log("Blackboard Distane Condition", "Target is null, can't perform distance check. Will never satisfy.", TafraDebugger.LogType.Error);
                return false;
            }

            float curSqrDistance = (value.transform.position - agentTransform.position).sqrMagnitude;
            return ZHelper.IsNumberRelationValid(curSqrDistance, sqrDistance, relation);
        }
    }
}