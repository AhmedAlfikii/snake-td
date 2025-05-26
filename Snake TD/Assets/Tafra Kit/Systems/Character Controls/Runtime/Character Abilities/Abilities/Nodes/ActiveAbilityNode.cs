using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [GraphNodeName("Active"), RemoveFromSearchMenu()]
    public class ActiveAbilityNode : AbilityNode
    {
        public override bool HasInputPort => false;
        public override bool HasOutputPort => true;
        public override bool MultiInputs => false;
        public override bool MultiOutputs => false;

        protected override BTNodeState OnUpdate()
        {
            if(children.Count > 0 && children[0] != null)
                return children[0].Update();

            return BTNodeState.Success;
        }
    }
}