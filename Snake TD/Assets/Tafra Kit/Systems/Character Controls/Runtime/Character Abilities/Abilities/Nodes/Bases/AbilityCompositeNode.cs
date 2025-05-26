using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    public abstract class AbilityCompositeNode : AbilityNode
    {
        public override bool HasInputPort => true;
        public override bool HasOutputPort => true;
        public override bool MultiInputs => true;
        public override bool MultiOutputs => true;

        public AbilityCompositeNode(AbilityCompositeNode other) : base(other)
        {

        }
        public AbilityCompositeNode()
        {

        }
    }
}