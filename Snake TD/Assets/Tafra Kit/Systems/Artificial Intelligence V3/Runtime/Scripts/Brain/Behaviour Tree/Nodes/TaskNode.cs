using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;

namespace TafraKit.Internal.AI3
{
    public abstract class TaskNode : AIBTNode
    {
        public override bool HasInputPort => true;
        public override bool HasOutputPort => false;
        public override bool MultiInputs => true;
        public override bool MultiOutputs => false;
    }
}