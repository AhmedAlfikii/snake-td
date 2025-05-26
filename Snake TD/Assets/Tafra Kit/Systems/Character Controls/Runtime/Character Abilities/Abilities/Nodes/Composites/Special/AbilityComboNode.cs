using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Composites/Special/Combo"), GraphNodeName("Combo")]
    public class AbilityComboNode : AbilityCompositeNode
    {
        [SerializeField] private float maxInterval = 0.15f;

        private int targetChildIndex;
        private float lastEndTime = -99999;

        public AbilityComboNode(AbilityComboNode other) : base(other)
        {
            maxInterval = other.maxInterval;
        }
        public AbilityComboNode()
        {

        }

        protected override void OnStart()
        {
            if(Time.time < lastEndTime + maxInterval)
                targetChildIndex++;     //Go to the next node in the combo since we're still within the allowed interval.
            else
                targetChildIndex = 0;   //Restart the combo since more time has passed than allowed.

            //If all the nodes of the combo have been used, then restart.
            if(targetChildIndex >= children.Count)
                targetChildIndex = 0;
        }
        protected override BTNodeState OnUpdate()
        {
            if (children.Count > targetChildIndex)
            {
                BTNode targetChild = children[targetChildIndex];
                BTNodeState childState = targetChild.Update();

                return childState;
            }

            return BTNodeState.Success;
        }
        protected override void OnEnd()
        {
            lastEndTime = Time.time;
            base.OnEnd();
        }
        protected override BTNode CloneContent()
        {
            AbilityComboNode clonedNode = new AbilityComboNode(this);

            return clonedNode;
        }
    }
}