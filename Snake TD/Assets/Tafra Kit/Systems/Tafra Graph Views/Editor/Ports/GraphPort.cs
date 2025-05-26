using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.GraphViews
{
    public class GraphPort : Port
    {
        protected class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;

            private List<Edge> m_EdgesToCreate;

            private List<GraphElement> m_EdgesToDelete;
            public Action<Edge> OnDroppedOutsidePort;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                OnDroppedOutsidePort?.Invoke(edge);
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                {
                    foreach (Edge connection in edge.input.connections)
                    {
                        if (connection != edge)
                        {
                            m_EdgesToDelete.Add(connection);
                        }
                    }
                }

                if (edge.output.capacity == Capacity.Single)
                {
                    foreach (Edge connection2 in edge.output.connections)
                    {
                        if (connection2 != edge)
                        {
                            m_EdgesToDelete.Add(connection2);
                        }
                    }
                }

                if (m_EdgesToDelete.Count > 0)
                {
                    graphView.DeleteElements(m_EdgesToDelete);
                }

                List<Edge> edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                {
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                }

                foreach (Edge item in edgesToCreate)
                {
                    graphView.AddElement(item);
                    edge.input.Connect(item);
                    edge.output.Connect(item);
                }
            }
        }

        public Action<Edge> OnEdgeDropperOutsidePort;

        protected Rect BaseRect
        {
            get
            {
                Rect rect = layout;
                return new Rect(0f, 0f, rect.width, rect.height);
            }
        }

        public GraphPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        /// <summary>
        /// Check if point is on top of port. Used for selection and hover.
        /// </summary>
        /// <param name="localPoint"></param>
        /// <returns>True if the point is over the port.</returns>
        public override bool ContainsPoint(Vector2 localPoint)
        {
            Rect myRect = direction == Direction.Input ? m_ConnectorBox.layout : m_ConnectorBox.layout;
            Rect rect2;
            if (direction == Direction.Input)
            {
                rect2 = new Rect(0f - myRect.xMin, 0f - myRect.yMin, myRect.width + myRect.xMin, BaseRect.height);
                rect2.width += m_ConnectorText.layout.xMin - myRect.xMax;
            }
            else
            {
                rect2 = new Rect(0f, 0f - myRect.yMin, BaseRect.width - myRect.xMin, BaseRect.height);
                float num = myRect.xMin - m_ConnectorText.layout.xMax;
                rect2.xMin -= num;
                rect2.width += num;
            }

            return rect2.Contains(this.ChangeCoordinatesTo(m_ConnectorBox, localPoint));
        }
        protected virtual void EdgeDropperOutsidePort(Edge edge)
        {
            OnEdgeDropperOutsidePort?.Invoke(edge);
        }
    }
}