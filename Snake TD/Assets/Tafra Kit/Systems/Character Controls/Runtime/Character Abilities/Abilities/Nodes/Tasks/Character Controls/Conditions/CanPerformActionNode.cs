using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/Conditions/Can Perform Action"), GraphNodeName("Can Perform Action")]

    public class CanPerformActionNode : AbilityTaskNode
    {
        private ICharacterController controller;

        public CanPerformActionNode(CanPerformActionNode other) : base(other)
        {
            controller = other.controller;
        }
        public CanPerformActionNode()
        {

        }

        protected override void OnInitialize()
        {
            controller = actor as ICharacterController;
        }

        protected override BTNodeState OnUpdate()
        {
            if (controller != null)
            {
                if(controller.CanPerformNewAction)
                    return BTNodeState.Success;
                else return BTNodeState.Failure;
            }
            
            return BTNodeState.Failure;
        }
        protected override BTNode CloneContent()
        {
            CanPerformActionNode clonedNode = new CanPerformActionNode(this);

            return clonedNode;
        }
    }
}