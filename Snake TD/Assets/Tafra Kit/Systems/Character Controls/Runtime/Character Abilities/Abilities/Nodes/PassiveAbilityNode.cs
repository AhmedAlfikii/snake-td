using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    /// <summary>
    /// The children of this node will be updated in parallel, infinitely.
    /// </summary>
    [GraphNodeName("Passive"), RemoveFromSearchMenu()]
    public class PassiveAbilityNode : AbilityNode
    {
        public override bool HasInputPort => false;
        public override bool HasOutputPort => true;
        public override bool MultiInputs => false;
        public override bool MultiOutputs => true;

        protected override BTNodeState OnUpdate()
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Update();
            }

            return BTNodeState.Running;
        }
    }
}