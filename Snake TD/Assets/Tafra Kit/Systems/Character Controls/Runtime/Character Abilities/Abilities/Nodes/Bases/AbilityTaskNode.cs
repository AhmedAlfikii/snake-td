using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    public abstract class AbilityTaskNode : AbilityNode
    {
        public override bool HasInputPort => true;
        public override bool HasOutputPort => false;
        public override bool MultiInputs => false;
        public override bool MultiOutputs => false;

        public AbilityTaskNode(AbilityTaskNode other) : base(other)
        {

        }
        public AbilityTaskNode()
        {

        }
    }
}