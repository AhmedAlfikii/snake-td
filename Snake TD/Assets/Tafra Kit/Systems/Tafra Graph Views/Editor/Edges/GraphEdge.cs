using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TafraKitEditor.GraphViews
{
    public class GraphEdge : Edge
    {
        public Action<GraphEdge> OnEdgeSelected;
        public Action<GraphEdge> OnEdgeUnselected;

        public override void OnSelected()
        {
            base.OnSelected();
            OnEdgeSelected?.Invoke(this);
        }
        public override void OnUnselected()
        {
            base.OnUnselected();
            OnEdgeUnselected?.Invoke(this);
        }
    }
}
