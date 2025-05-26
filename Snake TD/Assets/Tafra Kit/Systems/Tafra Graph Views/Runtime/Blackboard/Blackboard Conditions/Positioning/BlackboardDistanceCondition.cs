using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.GraphViews
{
    [SearchMenuItem("Positioning/Distance")]
    public class BlackboardDistanceCondition : BlackboardCondition
    {
        [SerializeField] private BlackboardGameObjectGetter target;
        [SerializeField] private float distance;
        [SerializeField] private NumberRelation relation = NumberRelation.GreaterThan;
        [SerializeField] private string prefixMsg;

        private Transform agentTransform;
        private float sqrDistance;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            agentTransform = actor.transform;

            sqrDistance = distance * distance;

            target.Initialize(blackboardCollection);
        }
        protected override bool PerformCheck()
        {
            GameObject value = target.Value;

            if(value == null)
            {
                TafraDebugger.Log("Blackboard Distane Condition", "Target is null, can't perform distance check. Will never satisfy.", TafraDebugger.LogType.Error);
                return false;
            }

            float curSqrDistance = (value.transform.position - agentTransform.position).sqrMagnitude;
            //Debug.Log($"Brain - {prefixMsg} Cur Distance: " + (value.transform.position - agentTransform.position).magnitude + ". " + ZHelper.IsNumberRelationValid(curSqrDistance, sqrDistance, relation));
            return ZHelper.IsNumberRelationValid(curSqrDistance, sqrDistance, relation);
        }
    }
}