using System;
using System.Collections.Generic;
using TafraKit;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using TafraKitEditor.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    public class StateMachineGraphView : BrainGraphView
    {
        private IStateMachine stateMachine;
        private SerializedProperty stateMachineProperty;

        protected override Type DefaultNodeEditor => typeof(StateNodeEditor);

        public StateMachineGraphView(SerializedObject serializedObject, SerializedProperty stateMachineProperty, IStateMachine stateMachine, string graphName, BrainWindow window) : base(serializedObject, window)
        {
            this.stateMachine = stateMachine;
            this.stateMachineProperty = stateMachineProperty;

            name = graphName;

            searchProvider.Initialize(typeof(State), "States", OnSearchItemSelected);

            InitializeView();
        }

        public void AssignStateMachine(IStateMachine stateMachine = null)
        {
            this.stateMachine = stateMachine;
        }

        private void OnSearchItemSelected(Type type, SearchWindowContext context)
        {
            var windowMousePosition = this.ChangeCoordinatesTo(this, context.screenMousePosition - window.position.position);
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);

            CreateState(type, graphMousePosition);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            foreach(Port endPort in ports)
            {
                if(endPort.direction == startPort.direction)
                    continue;

                if(ArePortsConnected(startPort, endPort))
                    continue;

                compatiblePorts.Add(endPort);
            }

            return compatiblePorts;
        }
        private bool ArePortsConnected(Port startPort, Port endPort)
        {
            foreach(var connection in startPort.connections)
            {
                if(connection.input == endPort || connection.output == endPort)
                    return true;
            }

            return false;
        }

        protected override void PopulateGraph()
        {
            graphViewChanged -= OnGraphViewChanged;
            
            DeleteElements(graphElements);
            nodeEditorByNodeDict.Clear();

            for(int i = 0; i < stateMachine.States.Count; i++)
            {
                State state = stateMachine.States[i];
                AddNodeToGraph(state);
            }

            for(int i = 0; i < stateMachine.States.Count; i++)
            {
                State state = stateMachine.States[i];

                for(int j = 0; j < state.Transitions.Count; j++)
                {
                    DrawTransition(state.Transitions[j]);
                }
            }

            graphViewChanged += OnGraphViewChanged;
        }
        protected override void OnEdgeCreated(Edge edge)
        {
            CreateTransition(edge);
        }
        protected override void OnEdgeRemoved(Edge edge)
        {
            if(edge.output == null || edge.input == null)
                return;

            #region Fire an unselected action in case it was selected before deletion
            SerializedProperty transitionProperty = GetTransitionProperty(edge as StateTransitionEdge);

            if(transitionProperty != null)
                OnExposablePropertyUnselected.Invoke(transitionProperty);
            #endregion

            StateNodeEditor outputStateNode = edge.output.node as StateNodeEditor;
            StateNodeEditor inputStateNode = edge.input.node as StateNodeEditor;

            State outputState = outputStateNode.Node as State;
            State inputState = inputStateNode.Node as State;

            for(int i = 0; i < outputState.Transitions.Count; i++)
            {
                StateTransition transition = outputState.Transitions[i];

                if (transition.ToState == inputState)
                {
                    outputState.Transitions.RemoveAt(i);
                    break;
                }    
            }
        }
        protected override void OnNodeRemoved(GraphNodeEditor editorNode)
        {
            State state = (State)editorNode.Node;

            //Fire an unselected action in case it was selected before deletion
            SerializedProperty stateProperty = GetStateProperty(editorNode.Node as State);
            if(stateProperty != null)
                OnExposablePropertyUnselected?.Invoke(stateProperty);

            //Remove transitions to this state that are coming out of other states
            //TODO: maybe optimize this? remove the need to loop over all of the state machine's states by storing the coming transitions somewhere and access them by state id (?).
            for(int i = 0; i < stateMachine.States.Count; i++)
            {
                var s = stateMachine.States[i];

                for(int j = 0; j < s.Transitions.Count; j++)
                {
                    var transition = s.Transitions[j];

                    if(transition.ToState == state)
                    {
                        s.Transitions.RemoveAt(j);
                        break;
                    }
                }
            }

            //Remove the state from the state machine.
            stateMachine.States.Remove(state);

            //Repopuplate the graph to clean up edges that represented removed transitions.
            //We'll have to delay the call because this function (OnNodeRemoved) is being called from inside the OnGraphChange callback,
            //and we don't want to conflict with the changes it will do.
            EditorApplication.delayCall += () =>
            {
                PopulateGraph();
            };
        }
        protected override void OnGraphElementSelected(GraphElement element)
        {
            if(element is StateNodeEditor nodeEditor)
            {
                State state = nodeEditor.Node as State;
                SerializedProperty stateProperty = GetStateProperty(state);

                if(stateProperty != null)
                    OnExposablePropertySelected?.Invoke(stateProperty, ObjectNames.NicifyVariableName(state.GetType().Name));
            }
            else if (element is StateTransitionEdge edge)
            {
                SerializedProperty transitionProperty = GetTransitionProperty(edge);

                if (transitionProperty != null)
                    OnExposablePropertySelected?.Invoke(transitionProperty, "Transition");
            }
        }
        protected override void OnGraphElementUnselected(GraphElement element)
        {
            if(element is StateNodeEditor nodeEditor)
            {
                SerializedProperty stateProperty = GetStateProperty(nodeEditor.Node as State);

                if(stateProperty != null)
                    OnExposablePropertyUnselected?.Invoke(stateProperty);
            }
            else if(element is StateTransitionEdge edge)
            {
                SerializedProperty transitionProperty = GetTransitionProperty(edge);

                if(transitionProperty != null)
                    OnExposablePropertyUnselected.Invoke(transitionProperty);
            }
        }
        protected override void OnGraphElementOpenRequest(GraphElement element)
        {
            if(element is SubstateMachineStateNodeEditor subMachineEditor)
            {
                SubstateMachineState substateMachineState = subMachineEditor.Node as SubstateMachineState;
                SerializedProperty stateProperty = GetStateProperty(subMachineEditor.Node as State);
                SerializedProperty externalStateMachineProperty = stateProperty.FindPropertyRelative("externalStateMachine");
                SerializedProperty internalStateMachineProperty = stateProperty.FindPropertyRelative("internalStateMachine");

                SerializedProperty availableStateMachineProperty;
                SerializedObject availableSerializedObject;
                if(externalStateMachineProperty.objectReferenceValue != null)
                {
                    availableSerializedObject = new SerializedObject(externalStateMachineProperty.objectReferenceValue);
                    availableStateMachineProperty = availableSerializedObject.FindProperty("stateMachine");
                }
                else
                {
                    availableSerializedObject = serializedObject;
                    availableStateMachineProperty = internalStateMachineProperty;
                }

                StateMachineGraphView graph = new StateMachineGraphView(availableSerializedObject, availableStateMachineProperty, substateMachineState.AvailableStateMachine, substateMachineState.Name, window);

                OnOpenNestedViewRequest?.Invoke(graph);
            }
            else if(element is BehaviourTreeStateNodeEditor behaviourTreeEditor)
            {
                BehaviourTreeState behaviourTreeState = behaviourTreeEditor.Node as BehaviourTreeState;
                SerializedProperty behaviourTreeProperty = GetStateProperty(behaviourTreeEditor.Node as State);
                SerializedProperty externalTreeProperty = behaviourTreeProperty.FindPropertyRelative("externalBehaviourTree");
                SerializedProperty internalTreeProperty = behaviourTreeProperty.FindPropertyRelative("internalBehaviourTree");

                SerializedProperty availableTreeProperty;
                SerializedObject availableSerializedObject;
                if(externalTreeProperty.objectReferenceValue != null)
                {
                    availableSerializedObject = new SerializedObject(externalTreeProperty.objectReferenceValue);
                    availableTreeProperty = availableSerializedObject.FindProperty("behaviourTree");
                }
                else
                {
                    availableSerializedObject = serializedObject;
                    availableTreeProperty = internalTreeProperty;
                }

                BehaviourTreeGraphView graph = new BehaviourTreeGraphView(availableSerializedObject, availableTreeProperty, behaviourTreeState.AvailableBehaviourTree, behaviourTreeState.Name, window);

                OnOpenNestedViewRequest?.Invoke(graph);
            }
        }
        protected override void NodeRenameRequest(GraphNodeEditor nodeEditor, string oldName, string newName)
        {
            Undo.RecordObject(serializedObject.targetObject, "Rename Node");

            nodeEditor.Node.Name = newName;
            nodeEditor.RefreshDisplayName();

            EditorUtility.SetDirty(serializedObject.targetObject);
        }

        private void CreateState(Type stateType, Vector2 position = default)
        {
            if(stateMachine == null)
            {
                TafraDebugger.Log("State Machine Graph", "Can't create state while no state machine is active.", TafraDebugger.LogType.Error);
                return;
            }

            State newState = Activator.CreateInstance(stateType) as State;

            newState.Position = new Rect(position, Vector2.zero);

            newState.SetHoldingObject(stateMachineProperty.serializedObject.targetObject);

            Undo.RecordObject(serializedObject.targetObject, "Create State");
            
            stateMachine.States.Add(newState);

            StateNodeEditor nodeEditor = AddNodeToGraph(newState) as StateNodeEditor;

            if (ghostEdge != null)
            {
                if (ghostEdge.input == null)
                    ghostEdge.input = nodeEditor.InputPort;
                else if (ghostEdge.output == null)
                    ghostEdge.output = nodeEditor.OutputPort;

                StateTransition transition = CreateTransition(ghostEdge);
                DrawTransition(transition);
                ghostEdge = null;
            }

            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        private StateTransition CreateTransition(Edge edge)
        {
            StateNodeEditor outputStateNode = edge.output.node as StateNodeEditor;
            StateNodeEditor inputStateNode = edge.input.node as StateNodeEditor;

            State outputState = outputStateNode.Node as State;
            State inputState = inputStateNode.Node as State;

            StateTransition transition = new StateTransition(outputState, inputState);

            outputState.Transitions.Add(transition);

            return transition;
        }
        private void DrawTransition(StateTransition transition)
        {
            StateNodeEditor outputStateEditor = nodeEditorByNodeDict[transition.FromState] as StateNodeEditor;
            StateNodeEditor inputStateEditor = nodeEditorByNodeDict[transition.ToState] as StateNodeEditor;

            GraphEdge edge = outputStateEditor.OutputPort.ConnectTo<StateTransitionEdge>(inputStateEditor.InputPort);

            AddElement(edge);

            edge.OnEdgeSelected = ElementSelected;
            edge.OnEdgeUnselected = ElementUnselected;
        }

        private SerializedProperty GetStateProperty(State state)
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return null;

            serializedObject.Update();

            int stateIndex = stateMachine.States.IndexOf(state);

            if(stateIndex != -1)
                return stateMachineProperty.FindPropertyRelative("states").GetArrayElementAtIndex(stateIndex);

            return null;
        }
        private SerializedProperty GetTransitionProperty(StateTransitionEdge edge)
        {
            if (edge == null)
                return null;

            if (edge.output == null || edge.input == null)
                return null;

            StateNodeEditor outputNodeEditor = edge.output.node as StateNodeEditor;
            StateNodeEditor inputNodeEditor = edge.input.node as StateNodeEditor;

            if(outputNodeEditor == null || inputNodeEditor == null)
                return null;

            State outputState = outputNodeEditor.Node as State;
            State inputState = inputNodeEditor.Node as State;

            if(outputState == null)
                return null;

            if(serializedObject == null || serializedObject.targetObject == null)
                return null;

            serializedObject.Update();

            for(int i = 0; i < outputState.Transitions.Count; i++)
            {
                var transition = outputState.Transitions[i];

                if(transition.ToState == inputState)
                {
                    int stateIndex = stateMachine.States.IndexOf(outputState);

                    if(stateIndex != -1)
                        return stateMachineProperty.FindPropertyRelative("states").GetArrayElementAtIndex(stateIndex).FindPropertyRelative("transitions").GetArrayElementAtIndex(i);
                }
            }

            return null;
        }

    }
}
