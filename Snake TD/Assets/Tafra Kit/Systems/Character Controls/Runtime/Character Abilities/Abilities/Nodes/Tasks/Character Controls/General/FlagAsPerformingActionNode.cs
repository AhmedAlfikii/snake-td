using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/General/Performing Action"), GraphNodeName("Performing Action")]
    public class FlagAsPerformingActionNode : AbilityTaskNode
    {
        [SerializeField] private string actionName;
        [SerializeField] private bool failIfCantPerform = true;

        private ICharacterController controller;
        private bool fail;
        private int actionHash;

        public FlagAsPerformingActionNode(FlagAsPerformingActionNode other) : base(other)
        {
            actionName = other.actionName;
            failIfCantPerform = other.failIfCantPerform;
            controller = other.controller;
            fail = other.fail;
            actionHash = other.actionHash;
        }
        public FlagAsPerformingActionNode()
        {

        }

        protected override void OnInitialize()
        {
            controller = actor as ICharacterController;
            actionHash = Animator.StringToHash(actionName);
        }
        protected override void OnStart()
        {
            fail = false;
             
            if(controller != null)
            {
                bool startedPerforming = controller.StartPerformingAction(actionHash);

                if(failIfCantPerform && !startedPerforming)
                    fail = true;
            }
            else
                fail = true;
        }
        protected override BTNodeState OnUpdate()
        {
            if(fail)
                return BTNodeState.Failure;
            else
                return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            if (controller != null && !fail)
                controller.FinishPerformingAction(actionHash);
        }
        protected override BTNode CloneContent()
        {
            FlagAsPerformingActionNode clonedNode = new FlagAsPerformingActionNode(this);

            return clonedNode;
        }
    }
}