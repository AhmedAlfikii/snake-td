using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Debugging/Distance Debug"), GraphNodeName("Distance Debug", "Distance Debug")]
    public class DistanceDebugState : ActionState
    {
        [SerializeField] private BlackboardGameObjectGetter target;

        protected override void OnInitialize()
        {
            target.Initialize(agent.BlackboardCollection);
        }

        public override void Update()
        {
            GameObject value = target.Value;
            if(value == null)
            {
                Debug.Log($"No target, can't calcualte distance.");
                return;
            }

            Debug.Log($"Distance is: {Vector3.Distance(agent.transform.position, value.transform.position)}.");
        }
    }
}