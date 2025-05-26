using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(ExitState))]
    public class ExitStateNodeEditor : MandatoryStateNodeEditor
    {
        protected override bool HasInputPort => true;
        protected override bool HasOutputPort => false;

        public ExitStateNodeEditor(State state) : base(state)
        {
            AddToClassList("exitStateNode");
        }
        protected override void GetTypeDisplayNames()
        {
            typeDisplayName = typeShortDisplayName = "Exit";
        }
    }
}
