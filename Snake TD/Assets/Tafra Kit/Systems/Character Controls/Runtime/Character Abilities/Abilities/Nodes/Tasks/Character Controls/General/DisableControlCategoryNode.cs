using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/General/Disable Control Category"), GraphNodeName("Disable Control Category")]
    public class DisableControlCategoryNode : AbilityTaskNode
    {
        [SerializeField] private TafraString category;
        [Tooltip("If true, all the control categories will be disabled. The above category field will not be used.")]
        [SerializeField] private bool disableAll;
        [SerializeField] private string togglerID = "disableControlCategoryNode";

        private ICharacterController controller;

        public DisableControlCategoryNode(DisableControlCategoryNode other) : base(other)
        {
            category = other.category;
            disableAll = other.disableAll;
            togglerID = other.togglerID;
            controller = other.controller;
        }
        public DisableControlCategoryNode()
        {

        }

        protected override void OnInitialize()
        {
            controller = actor as ICharacterController;
        }
        protected override void OnStart()
        {
            if(controller != null)
            {
                if (!disableAll)
                    controller.ToggleControlCategory(category.Value, false, togglerID);
                else
                    controller.ToggleAllControlCategories(false, togglerID);
            }
        }
        protected override BTNodeState OnUpdate()
        {
                return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            if (controller != null)
                if(!disableAll)
                    controller.ToggleControlCategory(category.Value, true, togglerID);
                else
                    controller.ToggleAllControlCategories(true, togglerID);
        }
        protected override BTNode CloneContent()
        {
            DisableControlCategoryNode clonedNode = new DisableControlCategoryNode(this);

            return clonedNode;
        }
    }
}