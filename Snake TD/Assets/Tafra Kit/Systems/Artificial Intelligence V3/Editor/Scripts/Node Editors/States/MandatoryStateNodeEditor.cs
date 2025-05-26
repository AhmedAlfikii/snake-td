using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TafraKitEditor.AI3
{
    public abstract class MandatoryStateNodeEditor : StateNodeEditor
    {
        public MandatoryStateNodeEditor(State state) : base(state)
        {
            AddToClassList("mandatoryNode");

            capabilities = capabilities & ~Capabilities.Deletable;
        }
    }
}
