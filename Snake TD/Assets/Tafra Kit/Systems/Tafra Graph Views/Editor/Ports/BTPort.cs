using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.GraphViews
{
    public class BTPort : GraphPort
    {
        protected BTPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
            style.alignItems = Align.Stretch;
            style.width = 100;
            style.alignSelf = Align.Center;

            VisualElement connectorContainer = new VisualElement();
            connectorContainer.name = "connectorContainer";
            connectorContainer.style.flexGrow = 1;
            connectorContainer.style.alignItems = Align.Center;
            connectorContainer.style.justifyContent = Justify.Center;

            m_ConnectorBox.style.width = m_ConnectorBox.style.height = 10;

            Add(connectorContainer);
            connectorContainer.Add(m_ConnectorBox);
        }

        public static new BTPort Create<TEdge>(Orientation orientation, Direction direction, Capacity capacity, Type type) where TEdge : Edge, new()
        {
            DefaultEdgeConnectorListener listener = new DefaultEdgeConnectorListener();
            BTPort port = new BTPort(orientation, direction, capacity, type)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            port.AddManipulator(port.m_EdgeConnector);

            listener.OnDroppedOutsidePort = port.EdgeDropperOutsidePort;

            return port;
        }
    }
}