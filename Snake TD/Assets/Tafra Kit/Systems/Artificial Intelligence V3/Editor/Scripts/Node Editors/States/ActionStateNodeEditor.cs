using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEngine;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(ActionState))]
    public class ActionStateNodeEditor : StateNodeEditor
    {
        protected override bool HasInputPort => true;
        protected override bool HasOutputPort => true;

        public ActionStateNodeEditor(State state) : base(state)
        {
            AddToClassList("actionStateNode");
        }
    }
}
