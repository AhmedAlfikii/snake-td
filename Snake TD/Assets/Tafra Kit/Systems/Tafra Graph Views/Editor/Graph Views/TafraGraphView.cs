using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TafraKitEditor.GraphViews
{
    public abstract class TafraGraphView<T> : GraphView where T : Object
    {
        protected GraphWindow<T> window;
        protected SerializedObject serializedObject;
        protected object targetObject;
        /// <summary>
        /// The editor type for each of the node types.
        /// </summary>
        protected Dictionary<Type, Type> nodeEditorByTypeDict;  //Key: Node Type, Value: Editor Type
        protected Dictionary<GraphNode, GraphNodeEditor> nodeEditorByNodeDict;
        protected ClassSearchProvider searchProvider;
        protected Edge ghostEdge;

        protected abstract Type BaseNodeType { get; }
        /// <summary>
        /// The title that will appear in the search window.
        /// </summary>
        protected virtual string SearchWindowTitle { get => "Nodes"; }
        protected abstract Type DefaultNodeEditor { get; }

        public Action<GraphElement> OnElementSelected;
        public Action<GraphElement> OnElementUnselected;

        public Action<SerializedProperty, string> OnExposablePropertySelected;
        public Action<SerializedProperty> OnExposablePropertyUnselected;
        public Action<TafraGraphView<T>> OnOpenNestedViewRequest;
        public Action OnElementRemoved;

        public TafraGraphView(SerializedObject serializedObject, string graphName, GraphWindow<T> window)
        {
            this.serializedObject = serializedObject;
            this.window = window;
            targetObject = serializedObject.targetObject;
            name = graphName;

            SetupStyle();

            this.AddManipulator(new ContentZoomer() { maxScale = 3f });
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            Insert(0, new GridBackground());

            nodeEditorByTypeDict = new Dictionary<Type, Type>();
            nodeEditorByNodeDict = new Dictionary<GraphNode, GraphNodeEditor>();
            var types = TypeCache.GetTypesDerivedFrom(DefaultNodeEditor);
            for(int i = 0; i < types.Count; i++)
            {
                Type type = types[i];

                if(type.GetCustomAttribute<CustomNodeEditor>() is CustomNodeEditor customNodeEditor)
                    nodeEditorByTypeDict.TryAdd(customNodeEditor.NodeType, type);
            }

            searchProvider = ScriptableObject.CreateInstance<ClassSearchProvider>();
            searchProvider.Initialize(BaseNodeType, SearchWindowTitle, OnSearchItemSelected);

            this.nodeCreationRequest = OnNodeCreationRequest;

            this.canPasteSerializedData = (d) => { return true; };
            this.serializeGraphElements += CopyElements;
            this.unserializeAndPaste += UnserializeAndPaste;
        }

        private string CopyElements(IEnumerable<GraphElement> elements)
        {
            string json = "IMPLEMENT COPYING";

            return json;
        }

        private void UnserializeAndPaste(string operationName, string data)
        {
            Debug.Log(data);
        }

        public void InitializeView()
        {
            PopulateGraph();

            RegisterCallback<GeometryChangedEvent>(OnGeoChanged);
        }

        private void OnSearchItemSelected(Type type, SearchWindowContext context)
        {
            var windowMousePosition = this.ChangeCoordinatesTo(this, context.screenMousePosition - window.position.position);
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);

            CreateNodeAtPosition(type, graphMousePosition);
        }
        public void OnUndoRedoPerformed()
        {
            PopulateGraph();
        }
        protected virtual GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if(graphViewChange.movedElements != null)
            {
                Undo.RecordObject(serializedObject.targetObject, "Moved Elements");

                for(int i = 0; i < graphViewChange.movedElements.Count; i++)
                {
                    var element = graphViewChange.movedElements[i];

                    if(element is GraphNodeEditor editorNode)
                    {
                        editorNode.Node.Position = editorNode.GetPosition();
                        OnNodeMoved(editorNode);
                    }
                }

                EditorUtility.SetDirty(serializedObject.targetObject);
            }

            if(graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(serializedObject.targetObject, "Removed Elements");

                for(int i = 0; i < graphViewChange.elementsToRemove.Count; i++)
                {
                    var element = graphViewChange.elementsToRemove[i];

                    if(element is GraphNodeEditor editorNode)
                        OnNodeRemoved(editorNode);
                    else if(element is Edge edge)
                        OnEdgeRemoved(edge);
                }

                EditorUtility.SetDirty(serializedObject.targetObject);

                OnElementRemoved?.Invoke();
            }

            if(graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(serializedObject.targetObject, "Edges Created");

                for(int i = 0; i < graphViewChange.edgesToCreate.Count; i++)
                {
                    GraphEdge graphEdge = graphViewChange.edgesToCreate[i] as GraphEdge;

                    if(graphEdge != null)
                    {
                        graphEdge.OnEdgeSelected = ElementSelected;
                        graphEdge.OnEdgeUnselected = ElementUnselected;
                    }

                    ghostEdge = null;
                    OnEdgeCreated(graphViewChange.edgesToCreate[i]);
                }

                EditorUtility.SetDirty(serializedObject.targetObject);
            }

            return graphViewChange;
        }
        private void OnGeoChanged(GeometryChangedEvent evt)
        {
            FrameAll();

            UnregisterCallback<GeometryChangedEvent>(OnGeoChanged);
        }
        protected virtual void OnNodeCreationRequest(NodeCreationContext context)
        {
            ghostEdge = null;
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchProvider);
        }
        protected virtual void DisplaySearchWindowForGhostEdge() 
        {
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            SearchWindow.Open(new SearchWindowContext(mousePos), searchProvider);
        }
        protected virtual void OnGraphElementSelected(GraphElement element) { }
        protected virtual void OnGraphElementUnselected(GraphElement element) { }
        protected virtual void OnGraphElementOpenRequest(GraphElement element) { }
        protected virtual void OnNodeMoved(GraphNodeEditor editorNode) { }
        protected abstract void OnNodeRemoved(GraphNodeEditor editorNode);
        protected abstract void OnEdgeRemoved(Edge edge);
        protected abstract void OnEdgeCreated(Edge edge);
        protected void ElementSelected(GraphElement element)
        {
            OnGraphElementSelected(element);
            OnElementSelected?.Invoke(element);
        }
        protected void ElementUnselected(GraphElement element)
        {
            OnGraphElementUnselected(element);
            OnElementUnselected?.Invoke(element);
        }
        private void ElementOpenRequest(GraphElement element)
        {
            OnGraphElementOpenRequest(element);
        }
        private void EdgeDroppedOutsidePort(Edge edge)
        {
            ghostEdge = new Edge();
            ghostEdge.input = edge.input;
            ghostEdge.output = edge.output;

            DisplaySearchWindowForGhostEdge();
        }
        protected abstract void NodeRenameRequest(GraphNodeEditor nodeEditor, string oldName, string newName);

        protected abstract void PopulateGraph();
        protected abstract void CreateNodeAtPosition(Type nodeType, Vector2 graphPosition);
        private void SetupStyle()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Tafra Kit/Systems/Artificial Intelligence V3/Editor/USS & UXML/AIBrainWindow.uss");
            styleSheets.Add(styleSheet);

            style.flexGrow = 1;
        }
        protected virtual GraphNodeEditor CreateNodeEditor(GraphNode node)
        {
            Type editorType;
        
            //Look through the node type and its parents until an editor is found (otherwise use the default editor).
            Type curNodeType = node.GetType();
            do
            {
                nodeEditorByTypeDict.TryGetValue(curNodeType, out editorType);
                if(editorType == null)
                    curNodeType = curNodeType.BaseType;

            } while(editorType == null && curNodeType != null);

            if (editorType == null)
                editorType = DefaultNodeEditor;

            GraphNodeEditor nodeEditor = Activator.CreateInstance(editorType, node) as GraphNodeEditor;

            nodeEditor.OnNodeSelected = ElementSelected;
            nodeEditor.OnNodeUnselected = ElementUnselected;
            nodeEditor.OnOpenRequest = ElementOpenRequest;
            nodeEditor.OnEdgeDropperOutsidePort = EdgeDroppedOutsidePort;
            nodeEditor.OnRenameRequest = NodeRenameRequest;

            return nodeEditor;
        }
        protected GraphNodeEditor AddNodeToGraph(GraphNode node)
        {
            GraphNodeEditor nodeEditor = CreateNodeEditor(node);

            AddElement(nodeEditor);

            nodeEditor.SetPosition(node.Position);

            nodeEditorByNodeDict.Add(node, nodeEditor);

            return nodeEditor;
        }

        public virtual void OrganizeNodes()
        { 

        }
    }
}