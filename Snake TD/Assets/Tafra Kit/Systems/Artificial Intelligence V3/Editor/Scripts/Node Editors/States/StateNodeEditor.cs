using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using TafraKitEditor.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    public abstract class StateNodeEditor : GraphNodeEditor
    {
        protected State state;
        protected Port inputPort;
        protected Port outputPort;

        protected abstract bool HasInputPort { get; }
        protected abstract bool HasOutputPort { get; }

        public Port InputPort => inputPort;
        public Port OutputPort => outputPort;

        public StateNodeEditor(State state) : base(state, "Assets/Tafra Kit/Systems/Artificial Intelligence V3/Editor/USS & UXML/StateNodeEditor.uxml") 
        {
            this.state = state;

            CreatePorts();

            if(Application.isPlaying)
            {
                RegisterCallback<AttachToPanelEvent>((ev) =>
                {
                    state.LogEditorEvents = true;

                    if(state.IsPlaying)
                        OnStatePlay(state);
                    else
                        RemoveFromClassList("activeState");
                });
                RegisterCallback<DetachFromPanelEvent>((ev) =>
                {
                    state.LogEditorEvents = false;
                });

                state.EditorOnPlay = OnStatePlay;
                state.EditorOnConclude = OnStateConclude;
            }
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            bool addedLocalActions = false;

            if(outputPort != null)
            {
                evt.menu.AppendAction("Make Transition", a => DragTransitionEdge());
                addedLocalActions = true;
            }

            if(state is not MandatoryState)
            {
                evt.menu.AppendAction("Rename", a => Rename());
                addedLocalActions = true;
            }

            if (addedLocalActions)
                evt.menu.AppendSeparator();

            base.BuildContextualMenu(evt);
        }

        private void OnStatePlay(State state)
        {
            AddToClassList("activeState");
        }
        private void OnStateConclude(State state)
        {
            EditorApplication.delayCall += () =>
            {
                if (!state.IsPlaying)
                    RemoveFromClassList("activeState");
            };
        }
        private void CreatePorts()
        {
            if(HasInputPort)
                inputPort = CreatePort(Direction.Input);

            if(HasOutputPort)
                outputPort = CreatePort(Direction.Output);
        }
        private Port CreatePort(Direction direction)
        { 
            StatePort port = StatePort.Create<StateTransitionEdge>(Orientation.Horizontal, direction, Port.Capacity.Multi, typeof(bool));
            port.OnEdgeDropperOutsidePort = EdgeDroppedOutsidePort;
            Insert(0, port);
            port.RemoveFromClassList("port");
            return port;
        }
        private void DragTransitionEdge()
        {
            outputPort.SendEvent(new DragEvent(outputPort.GetGlobalCenter(), outputPort));
        }
        private class DragEvent : MouseDownEvent
        {
            public DragEvent(Vector2 mousePosition, VisualElement target)
            {
                this.mousePosition = mousePosition;
                this.target = target;
            }
        }
    }
}
