using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    public abstract class DecoratorNode : AIBTNode
    {
        protected BTNode child;
      
        public override bool HasInputPort => true;
        public override bool HasOutputPort => true;
        public override bool MultiInputs => true;
        public override bool MultiOutputs => false;

        protected override void OnStart()
        {
            if(children.Count > 0)
                child = children[0];
        }
    }
}