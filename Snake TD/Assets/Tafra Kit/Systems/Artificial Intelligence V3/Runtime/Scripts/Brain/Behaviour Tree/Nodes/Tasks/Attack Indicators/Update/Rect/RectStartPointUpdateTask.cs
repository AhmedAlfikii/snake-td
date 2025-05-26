using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Attack Indicators/Update/Rect/Rect Start Point Update"), GraphNodeName("Rect Start Point Update")]
    public class RectStartPointUpdateTask : TaskNode
    {
        [SerializeField] private bool startPointIsThisAgent;
        [SerializeField] private BlackboardDynamicPointGetter startPoint;
        [SerializeField] private BlackboardObjectGetter attackIndicator = new BlackboardObjectGetter();

        private RectAttackIndicator activeAttackIndicator;

        protected override void OnInitialize()
        {
            startPoint.Initialize(agent.BlackboardCollection);
            attackIndicator.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            activeAttackIndicator = attackIndicator.Value as RectAttackIndicator;
        }
        protected override BTNodeState OnUpdate()
        {

            Vector3 startPosition = startPointIsThisAgent ? agent.transform.position : startPoint.Value;

            activeAttackIndicator.RefreshAndMaintainEndPoint(startPosition);

            return BTNodeState.Running;
        }
    }
}