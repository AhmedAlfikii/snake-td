using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    public abstract class AbilityDecoratorNode : AbilityNode
    {
        protected AbilityNode child;
      
        public override bool HasInputPort => true;
        public override bool HasOutputPort => true;
        public override bool MultiInputs => true;
        public override bool MultiOutputs => false;

        public AbilityDecoratorNode(AbilityDecoratorNode other) : base(other)
        {

        }
        public AbilityDecoratorNode()
        {

        }

        protected override void OnStart()
        {
            if(children.Count > 0)
                child = children[0] as AbilityNode;
        }
    }
}