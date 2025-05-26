using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using TafraKitEditor.GraphViews;
using TafraKit.GraphViews;

namespace TafraKitEditor.AI3
{
    public class BehaviourTreeGraphView : BrainGraphView
    {
        private IBehaviourTree behaviourTree;
        private SerializedProperty behaviourTreeProperty;

        protected override Type DefaultNodeEditor => typeof(BTNodeEditor);

        public BehaviourTreeGraphView(SerializedObject serializedObject, SerializedProperty behaviourTreeProperty, IBehaviourTree behaviourTree, string graphName, BrainWindow window) : base(serializedObject, window)
        {
            this.behaviourTree = behaviourTree;
            this.behaviourTreeProperty = behaviourTreeProperty;

            name = graphName;

            searchProvider.Initialize(typeof(AIBTNode), "Nodes", OnSearchItemSelected);

            InitializeView();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            foreach (Port endPort in ports)
            {
                BTNodeEditor startBTNodeEditor = startPort.node as BTNodeEditor;
                BTNodeEditor endBTNodeEditor = endPort.node as BTNodeEditor;

                if (endPort.direction == startPort.direction || endPort.node == startPort.node)
                    continue;

                //Root nodes can only connect to composite nodes
                if ((startBTNodeEditor.Node is RootNode && endBTNodeEditor.Node is not CompositeNode)
                    || (endBTNodeEditor.Node is RootNode && startBTNodeEditor.Node is not CompositeNode))
                    continue;

                compatiblePorts.Add(endPort);
            }

            return compatiblePorts;
        }

        private void OnSearchItemSelected(Type type, SearchWindowContext context)
        {
            var windowMousePosition = this.ChangeCoordinatesTo(this, context.screenMousePosition - window.position.position);
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);

            CreateBTNode(type, graphMousePosition);
        }
        protected override void OnEdgeCreated(Edge edge)
        {
            CreateConnection(edge);
            ResortNodeConnections(((BTNodeEditor)edge.output.node).Node as AIBTNode);
        }
        protected override void OnEdgeRemoved(Edge edge)
        {
            if (edge.output == null || edge.input == null)
                return;

            //#region Fire an unselected action in case it was selected before deletion
            //SerializedProperty connectionProperty = GetConnectionProperty(edge as BTConnectionEdge);

            //if (connectionProperty != null)
            //    OnExposablePropertyUnselected.Invoke(connectionProperty);
            //#endregion

            BTNodeEditor outputNodeEditor = edge.output.node as BTNodeEditor;
            BTNodeEditor inputNodeEditor = edge.input.node as BTNodeEditor;

            AIBTNode outputNode = outputNodeEditor.Node as AIBTNode;
            AIBTNode inputNode = inputNodeEditor.Node as AIBTNode;

            for (int i = 0; i < outputNode.Children.Count; i++)
            {
                var child = outputNode.Children[i];

                if (child == inputNode)
                {
                    outputNode.Children.RemoveAt(i);
                    ResortNodeConnections(outputNode);
                    break;
                }
            }
        }
        protected override void OnNodeRemoved(GraphNodeEditor editorNode)
        {
            AIBTNode node = (AIBTNode)editorNode.Node;

            //TODO
            //Fire an unselected action in case it was selected before deletion
            SerializedProperty nodeProperty = GetNodeProperty(editorNode.Node as AIBTNode);
            if (nodeProperty != null)
                OnExposablePropertyUnselected?.Invoke(nodeProperty);

            //Remove connections to this node that are coming out of other nodes
            //TODO: maybe optimize this? remove the need to loop over all of the node machine's nodes by storing the coming connection somewhere and access them by node id (?).
            for (int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                var n = behaviourTree.Nodes[i];

                for (int j = 0; j < n.Children.Count; j++)
                {
                    var child = n.Children[j];

                    if (child == node)
                    {
                        n.Children.RemoveAt(j);
                        break;
                    }
                }
            }

            //Remove the node from the behaviour tree.
            behaviourTree.Nodes.Remove(node);

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
            if (element is BTNodeEditor nodeEditor)
            {
                AIBTNode node = nodeEditor.Node as AIBTNode;
                SerializedProperty nodeProperty = GetNodeProperty(node);

                if (nodeProperty != null)
                    OnExposablePropertySelected?.Invoke(nodeProperty, ObjectNames.NicifyVariableName(node.GetType().Name));
            }
            //else if (element is Edge edge)
            //{
            //    SerializedProperty connectionProperty = GetConnectionProperty(edge as BTConnectionEdge);

            //    if (connectionProperty != null)
            //        OnExposablePropertySelected.Invoke(connectionProperty, "Transition");
            //}
        }
        protected override void OnGraphElementUnselected(GraphElement element)
        {
            if (element is BTNodeEditor nodeEditor)
            {
                SerializedProperty nodeProperty = GetNodeProperty(nodeEditor.Node as AIBTNode);

                if (nodeProperty != null)
                    OnExposablePropertyUnselected?.Invoke(nodeProperty);
            }
            //else if (element is Edge edge)
            //{
            //    SerializedProperty connectionProperty = GetConnectionProperty(edge as BTConnectionEdge);

            //    if (connectionProperty != null)
            //        OnExposablePropertyUnselected.Invoke(connectionProperty);
            //}
        }
        protected override void OnGraphElementOpenRequest(GraphElement element)
        {
            if (element is BTNodeEditor btNodeEditor && btNodeEditor.Node is SubBehaviourTreeTaskNode subBehaviourTree)
            {
                SerializedProperty nodeProperty = GetNodeProperty(subBehaviourTree);
                SerializedProperty externalTreeProperty = nodeProperty.FindPropertyRelative("externalBehaviourTree");
                SerializedProperty internalTreeProperty = nodeProperty.FindPropertyRelative("internalBehaviourTree");

                SerializedProperty availableTreeProperty;
                SerializedObject availableSerializedObject;
                if (externalTreeProperty.objectReferenceValue != null)
                {
                    availableSerializedObject = new SerializedObject(externalTreeProperty.objectReferenceValue);
                    availableTreeProperty = availableSerializedObject.FindProperty("behaviourTree");
                }
                else
                {
                    availableSerializedObject = serializedObject;
                    availableTreeProperty = internalTreeProperty;
                }

                BehaviourTreeGraphView graph = new BehaviourTreeGraphView(availableSerializedObject, availableTreeProperty, subBehaviourTree.AvailableBehaviourTree, subBehaviourTree.Name, window);

                OnOpenNestedViewRequest?.Invoke(graph);
            }
        }
        protected override void OnNodeMoved(GraphNodeEditor editorNode)
        {
            List<AIBTNode> nodes = behaviourTree.Nodes;
            for (int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                BTNode node = nodes[nodeIndex];
                
                //Only composite nodes can have multiple children
                if (node is not CompositeNode)
                    continue;

                List<BTNode> children = node.Children;
                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];

                    if (child == editorNode.Node)
                    {
                        ResortNodeConnections(node);

                        //Don't look at this nodes connections anymore since we are already resorting them.
                        break;
                    }
                }
            }
        }
        protected override void NodeRenameRequest(GraphNodeEditor nodeEditor, string oldName, string newName)
        {
            Undo.RecordObject(serializedObject.targetObject, "Rename Node");

            nodeEditor.Node.Name = newName;
            nodeEditor.RefreshDisplayName();

            EditorUtility.SetDirty(serializedObject.targetObject);
        }

        protected override void PopulateGraph()
        {
            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);
            nodeEditorByNodeDict.Clear();

            for(int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                BTNode node = behaviourTree.Nodes[i];
                BTNodeEditor nodeEditor = AddNodeToGraph(node) as BTNodeEditor;
            }

            for (int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                BTNode node = behaviourTree.Nodes[i];

                for (int j = 0; j < node.Children.Count; j++)
                {
                    DrawConnection(node, node.Children[j], j);
                }
            }
            graphViewChanged += OnGraphViewChanged;
        }
        protected override void DisplaySearchWindowForGhostEdge()
        {
            if(ghostEdge.output != null)
            {
                //We are looking for a child.

                AIBTNode node = ((BTNodeEditor)ghostEdge.output.node).Node as AIBTNode;

                //Root nodes can only have composite nodes as children.
                if(node is RootNode)
                    searchProvider.SetNextShowBaseType(typeof(CompositeNode));
            }
            else if(ghostEdge.input != null)
            {
                //We are looking for a parent.

                AIBTNode node = ((BTNodeEditor)ghostEdge.input.node).Node as AIBTNode;

                //Task nodes can't be the parent of any nodes.
                searchProvider.AddNextShowExcludedType(typeof(TaskNode));
            }

            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            SearchWindow.Open(new SearchWindowContext(mousePos), searchProvider);
        }
        private void CreateBTNode(Type nodeType, Vector2 position = default)
        {
            if(behaviourTree == null)
            {
                TafraDebugger.Log("Behaviour Tree Graph", "Can't create node while no behaviour tree is active.", TafraDebugger.LogType.Error);
                return;
            }

            AIBTNode newNode = Activator.CreateInstance(nodeType) as AIBTNode;

            newNode.Position = new Rect(position, Vector2.zero);

            Undo.RecordObject(serializedObject.targetObject, "Create Node");

            behaviourTree.Nodes.Add(newNode);

            BTNodeEditor nodeEditor = AddNodeToGraph(newNode) as BTNodeEditor;

            if (ghostEdge != null)
            {
                if (ghostEdge.input == null)
                    ghostEdge.input = nodeEditor.InputPort;
                else if (ghostEdge.output == null)
                    ghostEdge.output = nodeEditor.OutputPort;

                BTNode inputNode = ((BTNodeEditor)ghostEdge.input.node).Node as BTNode;
                BTNode outputNode = ((BTNodeEditor)ghostEdge.output.node).Node as BTNode;

                CreateConnection(ghostEdge);
                DrawConnection(outputNode, inputNode, 0);
                ghostEdge = null;
            }

            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        private void CreateConnection(Edge edge)
        {
            BTNodeEditor outputNodeEditor = edge.output.node as BTNodeEditor;
            BTNodeEditor inputNodeEditor = edge.input.node as BTNodeEditor;

            BTNode outputNode = outputNodeEditor.Node as BTNode;
            BTNode inputNode = inputNodeEditor.Node as BTNode;

            outputNode.Children.Add(inputNode);
        }
        private void DrawConnection(BTNode from, BTNode to, int connectionIndex)
        {
            BTNodeEditor fromNodeEditor = nodeEditorByNodeDict[from] as BTNodeEditor;
            BTNodeEditor toNodeEditor = nodeEditorByNodeDict[to] as BTNodeEditor;

            GraphEdge edge = fromNodeEditor.OutputPort.ConnectTo<BTConnectionEdge>(toNodeEditor.InputPort);

            AddElement(edge);

            edge.OnEdgeSelected = ElementSelected;
            edge.OnEdgeUnselected = ElementUnselected;

            toNodeEditor.SetExecutionOrder(connectionIndex + 1);
        }

        private void ResortNodeConnections(BTNode node)
        {
            node.Children.Sort(SortByXPosition);

            List<BTNode> children = node.Children;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];

                BTNodeEditor toNode = nodeEditorByNodeDict[child] as BTNodeEditor;
                toNode.SetExecutionOrder(i + 1);
            }
        }

        private int SortByXPosition(BTNode node1, BTNode node2)
        {
            BTNodeEditor node1Editor = nodeEditorByNodeDict[node1] as BTNodeEditor;
            BTNodeEditor node2Editor = nodeEditorByNodeDict[node2] as BTNodeEditor;

            if (node1Editor.GetPosition().x > node2Editor.GetPosition().x)
                return 1;
            else
                return -1;
        }

        private SerializedProperty GetNodeProperty(AIBTNode node)
        {
            if(serializedObject == null || serializedObject.targetObject == null)
                return null;

            serializedObject.Update();
            
            int nodeIndex = behaviourTree.Nodes.IndexOf(node);
            
            if (nodeIndex != -1)
                return behaviourTreeProperty.FindPropertyRelative("nodes").GetArrayElementAtIndex(nodeIndex);

            return null;
        }

        //private SerializedProperty GetConnectionProperty(BTConnectionEdge edge)
        //{
        //    if (edge == null)
        //        return null;

        //    if (edge.output == null || edge.input == null)
        //        return null;

        //    BTNodeEditor outputNodeEditor = edge.output.node as BTNodeEditor;
        //    BTNodeEditor inputNodeEditor = edge.input.node as BTNodeEditor;

        //    if (outputNodeEditor == null || inputNodeEditor == null)
        //        return null;

        //    BTNode outputNode = outputNodeEditor.BrainNode as BTNode;
        //    BTNode inputNode = inputNodeEditor.BrainNode as BTNode;

        //    if (outputNode == null)
        //        return null;

        //    serializedObject.Update();

        //    for (int i = 0; i < outputNode.Connections.Count; i++)
        //    {
        //        var connection = outputNode.Connections[i];

        //        if (connection.ToNodeId == inputNode.GUID)
        //        {
        //            int connectionIndex = behaviourTree.Nodes.IndexOf(outputNode);

        //            if (connectionIndex != -1)
        //                return behaviourTreeProperty.FindPropertyRelative("nodes").GetArrayElementAtIndex(connectionIndex).FindPropertyRelative("connections").GetArrayElementAtIndex(i);
        //        }
        //    }

        //    return null;
        //}
    }
}