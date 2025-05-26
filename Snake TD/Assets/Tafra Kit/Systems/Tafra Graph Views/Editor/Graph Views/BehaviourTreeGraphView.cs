using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using TafraKit;

namespace TafraKitEditor.GraphViews
{
    public abstract class BehaviourTreeGraphView<T, TBTNode> : TafraGraphView<T> where T : Object where TBTNode : BTNode
    {
        private IBTNodesContainer<TBTNode> behaviourTree;
        private SerializedObject behaviourTreeSerializedObject;
        private SerializedProperty behaviourTreeSerializedProperty;
        
        protected override string SearchWindowTitle => "Nodes";
        protected override Type BaseNodeType => typeof(TBTNode);

        public BehaviourTreeGraphView(SerializedObject serializedObject, SerializedProperty behaviourTreeProperty, IBTNodesContainer<TBTNode> behaviourTree, string graphName, GraphWindow<T> window) : base(serializedObject, graphName, window)
        {
            this.behaviourTree = behaviourTree;
            behaviourTreeSerializedObject = serializedObject;
            behaviourTreeSerializedProperty = behaviourTreeProperty;

            InitializeView();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            foreach(Port endPort in ports)
            {
                BTNodeEditor startBTNodeEditor = startPort.node as BTNodeEditor;
                BTNodeEditor endBTNodeEditor = endPort.node as BTNodeEditor;

                if(endPort.direction == startPort.direction || endPort.node == startPort.node)
                    continue;

                if(!CanNodesConnect(startBTNodeEditor.Node, endBTNodeEditor.Node))
                    continue;

                compatiblePorts.Add(endPort);
            }

            return compatiblePorts;
        }

        protected override void OnEdgeCreated(Edge edge)
        {
            CreateConnection(edge);
            ResortNodeConnections(((BTNodeEditor)edge.output.node).Node as BTNode);
        }
        protected override void OnEdgeRemoved(Edge edge)
        {
            if(edge.output == null || edge.input == null)
                return;

            //#region Fire an unselected action in case it was selected before deletion
            //SerializedProperty connectionProperty = GetConnectionProperty(edge as BTConnectionEdge);

            //if (connectionProperty != null)
            //    OnExposablePropertyUnselected.Invoke(connectionProperty);
            //#endregion

            BTNodeEditor outputNodeEditor = edge.output.node as BTNodeEditor;
            BTNodeEditor inputNodeEditor = edge.input.node as BTNodeEditor;

            BTNode outputNode = outputNodeEditor.Node as BTNode;
            BTNode inputNode = inputNodeEditor.Node as BTNode;

            for(int i = 0; i < outputNode.Children.Count; i++)
            {
                var child = outputNode.Children[i];

                if(child == inputNode)
                {
                    outputNode.Children.RemoveAt(i);
                    ResortNodeConnections(outputNode);
                    break;
                }
            }
        }
        protected override void OnNodeRemoved(GraphNodeEditor editorNode)
        {
            TBTNode node = (TBTNode)editorNode.Node;

            //TODO
            //Fire an unselected action in case it was selected before deletion
            SerializedProperty nodeProperty = GetNodeProperty(editorNode.Node as TBTNode);
            if(nodeProperty != null)
                OnExposablePropertyUnselected?.Invoke(nodeProperty);

            //Remove connections to this node that are coming out of other nodes
            //TODO: maybe optimize this? remove the need to loop over all of the node machine's nodes by storing the coming connection somewhere and access them by node id (?).
            for(int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                var n = behaviourTree.Nodes[i];

                for(int j = 0; j < n.Children.Count; j++)
                {
                    var child = n.Children[j];

                    if(child == node)
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
            if(element is BTNodeEditor nodeEditor)
            {
                TBTNode node = nodeEditor.Node as TBTNode;
                SerializedProperty nodeProperty = GetNodeProperty(node);

                if(nodeProperty != null)
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
            if(element is BTNodeEditor nodeEditor)
            {
                SerializedProperty nodeProperty = GetNodeProperty(nodeEditor.Node as TBTNode);

                if(nodeProperty != null)
                    OnExposablePropertyUnselected?.Invoke(nodeProperty);
            }
            //else if (element is Edge edge)
            //{
            //    SerializedProperty connectionProperty = GetConnectionProperty(edge as BTConnectionEdge);

            //    if (connectionProperty != null)
            //        OnExposablePropertyUnselected.Invoke(connectionProperty);
            //}
        }
        protected override void OnNodeMoved(GraphNodeEditor editorNode)
        {
            //Re-order nodes.

            List<TBTNode> nodes = behaviourTree.Nodes;
            for(int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                BTNode node = nodes[nodeIndex];

                //No need to process the children of nodes that can't have multiple ones.
                if(!CanNodeHaveMultipleChildren(node))
                    continue;

                List<BTNode> children = node.Children;
                for(int i = 0; i < children.Count; i++)
                {
                    var child = children[i];

                    if(child == editorNode.Node)
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

            for(int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                BTNode node = behaviourTree.Nodes[i];

                for(int j = 0; j < node.Children.Count; j++)
                {
                    DrawConnection(node, node.Children[j], j);
                }
            }
            graphViewChanged += OnGraphViewChanged;
        }
        protected override void DisplaySearchWindowForGhostEdge()
        {
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            SearchWindow.Open(new SearchWindowContext(mousePos), searchProvider);
        }
        protected virtual bool CanNodesConnect(GraphNode startNode, GraphNode endNode)
        {
            return true;
        }
        protected abstract bool CanNodeHaveMultipleChildren(GraphNode node);
        protected override void CreateNodeAtPosition(Type nodeType, Vector2 position = default)
        {
            if(behaviourTree == null)
            {
                TafraDebugger.Log("Behaviour Tree Graph", "Can't create node while no behaviour tree is active.", TafraDebugger.LogType.Error);
                return;
            }

            TBTNode newNode = Activator.CreateInstance(nodeType) as TBTNode;

            newNode.Position = new Rect(position, Vector2.zero);

            Undo.RecordObject(serializedObject.targetObject, "Create Node");

            behaviourTree.Nodes.Add(newNode);

            BTNodeEditor nodeEditor = AddNodeToGraph(newNode) as BTNodeEditor;

            if(ghostEdge != null)
            {
                if(ghostEdge.input == null)
                    ghostEdge.input = nodeEditor.InputPort;
                else if(ghostEdge.output == null)
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

            if(!outputNode.MultiOutputs && outputNode.Children.Count > 1)
            {
                for (int i = 0; i < outputNode.Children.Count - 1; i++)
                {
                    outputNode.Children.RemoveAt(i);
                    i--;
                }

                PopulateGraph();
            }
            
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
            for(int i = 0; i < children.Count; i++)
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

            if(node1Editor.GetPosition().x > node2Editor.GetPosition().x)
                return 1;
            else
                return -1;
        }
        protected SerializedProperty GetNodeProperty(TBTNode node)
        {
            if(serializedObject == null || serializedObject.targetObject == null)
                return null;

            serializedObject.Update();

            int nodeIndex = behaviourTree.Nodes.IndexOf(node);

            if(nodeIndex != -1)
            {
                if (behaviourTreeSerializedProperty != null)
                    return behaviourTreeSerializedProperty.FindPropertyRelative("nodes").GetArrayElementAtIndex(nodeIndex);
                else
                    return behaviourTreeSerializedObject.FindProperty("nodes").GetArrayElementAtIndex(nodeIndex);
            }

            return null;
        }
        private Vector2 spacing = new Vector2(250, 200);

        public override void OrganizeNodes()
        {
            Undo.RecordObject(serializedObject.targetObject, "Organize Nodes");
         
            List<TBTNode> rootNodes = new List<TBTNode>();
            List<TBTNode> strayNodes = new List<TBTNode>();
            Dictionary<TBTNode, TBTNode> childByParent = new Dictionary<TBTNode, TBTNode>();

            //Extract root nodes and child nodes.
            for(int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                TBTNode n = behaviourTree.Nodes[i];

                //Store the children nodes.
                for (int childIndex = 0; childIndex < n.Children.Count; childIndex++)
                {
                    var child = n.Children[childIndex];

                    childByParent.TryAdd((TBTNode)child, n) ;
                }

                //Get the root nodes.
                if(!n.HasInputPort)
                {
                    rootNodes.Add(n);
                }
            }

            //Find stray nodes (nodes that don't have a parent.
            for (int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                TBTNode n = behaviourTree.Nodes[i];

                if(!childByParent.ContainsKey(n) && !rootNodes.Contains(n))
                {
                    strayNodes.Add(n);
                }
            }

            //Organize branch
            float startX = 0;
            for (int i = 0; i < rootNodes.Count; i++)
            {
                var rootNode = rootNodes[i];

                rootNode.Position = new Rect(startX, 0, rootNode.Position.width, rootNode.Position.height);
                
                OrganizeBranch(rootNode, startX, out float minX, out float maxX);
             
                startX = maxX + spacing.x * 2;  //More spacing since this is a root node.
            }

            PopulateGraph();
        }
        private void OrganizeBranch(BTNode rootNode, float startX, out float minX, out float maxX)
        { 
            int childrenCount = rootNode.Children.Count;

            minX = rootNode.Position.x;
            maxX = rootNode.Position.x;

            if(childrenCount == 0)
            {
                rootNode.Position = new Rect(startX, rootNode.Position.y, rootNode.Position.width, rootNode.Position.height);

                return;
            }

            bool evenChildrenCount = childrenCount % 2 == 0;

            float directChildrenMinX = Mathf.Infinity;
            float directChildrenMaxX = -Mathf.Infinity;

            float curX = startX;
            float curY = rootNode.Position.y + spacing.y;

            for(int i = 0; i < childrenCount; i++)
            {
                var childNode = rootNode.Children[i];

                childNode.Position = new Rect(curX, curY, childNode.Position.width, childNode.Position.height);

                OrganizeBranch(childNode, curX, out float nestedMinX, out float nestedMaxX);

                if (nestedMinX < minX)
                    minX = nestedMinX;
                if (nestedMaxX > maxX)
                    maxX = nestedMaxX;

                curX = nestedMaxX + spacing.x;
            }

            float width = maxX - minX;

            //Place the root.
            if(evenChildrenCount)
            {
                for(int i = 0; i < childrenCount; i++)
                {
                    var childNode = rootNode.Children[i];

                    float childX = childNode.Position.x;

                    if(childX < directChildrenMinX)
                        directChildrenMinX = childX;
                    if(childX > directChildrenMaxX)
                        directChildrenMaxX = childX;
                }

                float targetX = directChildrenMinX + (directChildrenMaxX - directChildrenMinX) / 2f;

                //Snap it to the closest child X if below threshold and all the other nodes are much further away, for a cleaner look.
                for(int i = 0; i < childrenCount; i++)
                {
                    var childNode = rootNode.Children[i];
                    float childX = childNode.Position.x;
                    float distance = Mathf.Abs(targetX - childX);
                    if(distance < spacing.x)
                    {
                        bool valid = true;
                        for(int j = 0; j < childrenCount; j++)
                        {
                            if(j == i)
                                continue;

                            var otherChildNode = rootNode.Children[j];
                            float distance2 = Mathf.Abs(targetX - otherChildNode.Position.x);
                            if(distance2 < distance * 2)
                            {
                                valid = false;
                                break;
                            }

                        }

                        if(!valid)
                            break;

                        targetX = childX;
                    }
                }

                rootNode.Position = new Rect(targetX, rootNode.Position.y, rootNode.Position.width, rootNode.Position.height);
            }
            else
            {
                int centerNodeIndex = Mathf.RoundToInt((childrenCount + 1) / 2f) - 1;

                rootNode.Position = new Rect(rootNode.Children[centerNodeIndex].Position.x, rootNode.Position.y, rootNode.Position.width, rootNode.Position.height);
            }
        }
    }
}