using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(EntryState))]
    public class EntryStateNodeEditor : MandatoryStateNodeEditor
    {
        protected override bool HasInputPort => false;
        protected override bool HasOutputPort => true;

        public EntryStateNodeEditor(State state) : base(state)
        {
            AddToClassList("entryStateNode");
        }
        protected override void GetTypeDisplayNames()
        {
            typeDisplayName = typeShortDisplayName = "Entry";
        }
    }
}
