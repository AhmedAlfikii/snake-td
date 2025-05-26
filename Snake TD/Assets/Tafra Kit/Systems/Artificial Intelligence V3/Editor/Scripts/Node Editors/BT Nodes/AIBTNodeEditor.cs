using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using TafraKitEditor.GraphViews;
using TafraKit.GraphViews;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    public class AIBTNodeEditor : BTNodeEditor
    {
        public AIBTNodeEditor(AIBTNode btNode) : base(btNode, "Assets/Tafra Kit/Systems/Artificial Intelligence V3/Editor/USS & UXML/BTNodeEditor.uxml") 
        {
        }

        private Port CreatePort(Direction direction, Port.Capacity capacity)
        {
            BTPort port = BTPort.Create<BTConnectionEdge>(Orientation.Vertical, direction, capacity, typeof(bool));
            port.OnEdgeDropperOutsidePort = EdgeDroppedOutsidePort;

            VisualElement connector = port.Q<VisualElement>("connector");
            connector.pickingMode = PickingMode.Position;

            port.Remove(port.Q<Label>("type"));

            if (direction == Direction.Input)
                inputContainer.Add(port);
            else if (direction == Direction.Output)
                outputContainer.Add(port);
            return port;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            bool addedLocalActions = false;
          
            if(btNode is not RootNode)
            {
                evt.menu.AppendAction("Rename", a => Rename());
                addedLocalActions = true;
            }

            if (addedLocalActions)
                evt.menu.AppendSeparator();

            base.BuildContextualMenu(evt);
        }
    }
}
