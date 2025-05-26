using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.GraphViews
{
    public class BTNodeEditor : GraphNodeEditor
    {
        protected BTNode btNode;
        protected Port inputPort;
        protected Port outputPort;
        protected Label executionOrderLabel;

        public Port InputPort => inputPort;
        public Port OutputPort => outputPort;

        public BTNodeEditor(BTNode btNode) : base(btNode, "Assets/Tafra Kit/Systems/Artificial Intelligence V3/Editor/USS & UXML/BTNodeEditor.uxml")
        {
            Initialize(btNode);
        }
        public BTNodeEditor(BTNode btNode, string uiFile) : base(btNode, uiFile) 
        {
            Initialize(btNode);
        }

        private void Initialize(BTNode btNode)
        {
            this.btNode = btNode;
            executionOrderLabel = this.Q<Label>("execution-order");

            CreatePorts();

            if(Application.isPlaying)
            {
                RegisterCallback<AttachToPanelEvent>((ev) =>
                {
                    btNode.LogEditorEvents = true;

                    if(btNode.IsStarted && btNode.State == BTNodeState.Running)
                        OnNodeStart(btNode);
                    else if(btNode.State == BTNodeState.Success)
                        OnNodeSuccess(btNode);
                    else if(btNode.State == BTNodeState.Failure)
                        OnNodeFailure(btNode);
                });
                RegisterCallback<DetachFromPanelEvent>((ev) =>
                {
                    btNode.LogEditorEvents = false;
                });

                btNode.EditorOnStart = OnNodeStart;
                btNode.EditorOnSuccess = OnNodeSuccess;
                btNode.EditorOnFailure = OnNodeFailure;
                btNode.EditorOnReset = OnNodeReset;
            }
        }

        private void OnNodeStart(BTNode node)
        {
            RemoveFromClassList("successNode");
            RemoveFromClassList("failureNode");
            AddToClassList("runningNode");
        }
        private void OnNodeSuccess(BTNode node)
        {
            RemoveFromClassList("runningNode");
            RemoveFromClassList("failureNode");
            AddToClassList("successNode");
        }
        private void OnNodeFailure(BTNode node)
        {
            RemoveFromClassList("runningNode");
            RemoveFromClassList("successNode");
            AddToClassList("failureNode");
        }
        private void OnNodeReset(BTNode node)
        {
            RemoveFromClassList("runningNode");
            RemoveFromClassList("successNode");
            RemoveFromClassList("failureNode");
        }

        private void CreatePorts()
        {
            if(btNode.HasInputPort)
                inputPort = CreatePort(Direction.Input, btNode.MultiInputs ? Port.Capacity.Multi : Port.Capacity.Single);

            if(btNode.HasOutputPort)
                outputPort = CreatePort(Direction.Output, btNode.MultiOutputs ? Port.Capacity.Multi : Port.Capacity.Single);
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

        public void SetExecutionOrder(int order)
        {
            if (executionOrderLabel != null)
                executionOrderLabel.text = order.ToString();
        }
    }
}
