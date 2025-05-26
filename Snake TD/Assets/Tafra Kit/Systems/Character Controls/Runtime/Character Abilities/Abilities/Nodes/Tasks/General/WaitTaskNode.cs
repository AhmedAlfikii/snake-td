using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/General/Wait"), GraphNodeName("Wait")]
    public class WaitTaskNode : AbilityTaskNode
    {
        [SerializeField] private BlackboardDynamicFloatGetter waitDuration = new BlackboardDynamicFloatGetter(1);

        [NonSerialized] private float endTime;

        public WaitTaskNode(WaitTaskNode otherWaitNode) : base(otherWaitNode)
        {
            waitDuration = new BlackboardDynamicFloatGetter(otherWaitNode.waitDuration);
            endTime = otherWaitNode.endTime;
        }
        public WaitTaskNode()
        {

        }

        protected override void OnInitialize()
        {
            waitDuration.Initialize(ability.BlackboardCollection);
        }

        protected override void OnStart()
        {
            endTime = Time.time + waitDuration.Value;
        }
        protected override BTNodeState OnUpdate()
        {
            if(Time.time < endTime)
                return BTNodeState.Running;

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            WaitTaskNode clonedNode = new WaitTaskNode(this);

            return clonedNode;
        }
    }
}