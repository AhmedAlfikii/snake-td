using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Collision Detection/Linecast To Point"), GraphNodeName("Linecast To Point", "Linecast To Point")]
    public class LinecastToPointTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter point = new BlackboardDynamicPointGetter();
        [SerializeField] private BlackboardVector3Getter offest = new BlackboardVector3Getter();
        [SerializeField] private BlackboardBoolGetter succeedOnCollision = new BlackboardBoolGetter();
        [SerializeField] private TafraLayerMask collisionLayers;

        private bool foundCollision;

        protected override void OnInitialize()
        {
            point.Initialize(agent.BlackboardCollection);
            offest.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            Vector3 offsetValue = offest.Value;
            Vector3 startPos = agent.transform.position + offsetValue;
            Vector3 endPos = point.Value + offsetValue;

            foundCollision = Physics.Linecast(startPos, endPos, collisionLayers.Value, QueryTriggerInteraction.Ignore);
        }
        protected override BTNodeState OnUpdate()
        {
            bool succeedOnCollisionValue = succeedOnCollision.Value;

            if ((succeedOnCollisionValue && foundCollision) || (!succeedOnCollisionValue && !foundCollision))
                return BTNodeState.Success;
            else
                return BTNodeState.Failure;
        }
    }
}