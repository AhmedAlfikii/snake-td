using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;
using System;
using Object = UnityEngine.Object;

namespace TafraKitEditor.GraphViews
{
    //Some information to keep in mind:
    //1- The currently dispalyed graph is alawys the last one in the nested graphs list if it has any graphs. If not it's always the main graph.
    //2- Opening a nested graph removes all the nested graphs after it.
    //3- Opening the main graph removes all nested graphs.
    public abstract class GraphWindow<TObject> : EditorWindow where TObject : Object
    {
        public enum PanelPlacement
        { 
            None,
            Left,
            Right,
            Top,
            Bottom,
            UpperRight,
            UpperLeft,
            LowerRight,
            LowerLeft
        }
        public enum PanelType
        { 
            Blackboard,
            SplitView
        }

        protected TObject containerObject;
        protected SerializedObject containerSerializedObject;
        protected VisualElement mainContent;
        protected VisualElement graphPanel;
        protected VisualElement graphContainer;
        protected GraphView activeGraph;
        protected GraphView mainGraph;
        protected ToolbarBreadcrumbs toolbarBreadcrumbs;
        protected List<GraphView> nestedGraphs = new List<GraphView>();
        protected List<Blackboard> activeBlackboards = new List<Blackboard>();
        protected SplitView ispectorSplitView;
        protected Button inspectorCollapseButton;
        protected Button organizeNodesButton;
        protected Blackboard inspectorBoardView;
        protected InspectorView inspectorView;
        protected bool inspectorBoardPositionSet;
        protected bool isInspectorSplitViewCollapsed;
        protected string lastContainerObjectPath;

        protected abstract string DefaultTitle { get; }
        protected abstract string DarkModeIconPath { get; }
        protected abstract string LightModeIconPath { get; }
        protected abstract PanelPlacement InspectorPlacement { get; }
        protected virtual PanelType InspectorType { get => PanelType.SplitView; }
        protected string ContainerObjectPathPrefsKey => $"GRAPH_{this.GetType()}_LAST_CONTAINER_OBJECT_PATH";

        #region Opening Window
        protected static void InitializeWindow(GraphWindow<TObject> window, TObject containerObject)
        {
            //If this window was opened through the tool bar and not by opening a specific brain, then initialize it with default name and icon.
            if (containerObject == null)
            {
                string iconPath = EditorGUIUtility.isProSkin ? window.DarkModeIconPath : window.LightModeIconPath;
                Texture icon = !string.IsNullOrEmpty(iconPath) ? EditorGUIUtility.Load(iconPath) as Texture : null;
                window.titleContent = new GUIContent(window.DefaultTitle, icon);
            }
            //If this window already has the selected object, then just focus it.
            else if(window.containerObject == containerObject)
                window.Focus();
            //If this window doesn't have the desired object, then assign it.
            else
                window.SetContainerObject(containerObject);
        }
        #endregion

        protected virtual void OnEnable()
        {
            inspectorBoardPositionSet = false;

            //If we have a container object, this means that this window had a container object before it got destroyed (on recompile), so we need to show it again.
            if(containerObject != null)
                RefreshContainerObject();
            else
            {
                string savedPath = EditorPrefs.GetString(ContainerObjectPathPrefsKey);
                if(!string.IsNullOrEmpty(savedPath))
                {
                    TObject savedObject = AssetDatabase.LoadAssetAtPath<TObject>(savedPath);
                    if(savedObject != null)
                        SetContainerObject(savedObject);
                }
            }
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        protected virtual void OnUndoRedoPerformed()
        {
            if(containerObject == null)
                return;

            if(activeGraph != null && activeGraph is TafraGraphView<TObject> activeTafraGraph)
                activeTafraGraph.OnUndoRedoPerformed();
        }
        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch(change)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void CreateGUI()
        {
            Toolbar toolbar = new Toolbar();
            toolbarBreadcrumbs = new ToolbarBreadcrumbs();
            toolbar.Add(toolbarBreadcrumbs);
          
            rootVisualElement.Add(toolbar);

            mainContent = new VisualElement();
            mainContent.name = "main-content";
            mainContent.style.flexGrow = 1;
            mainContent.style.flexDirection = FlexDirection.Row;

            rootVisualElement.Add(mainContent);

            graphPanel = new VisualElement();
            graphPanel.name = "graph-panel";
            graphPanel.style.flexGrow = 1;

            graphContainer = new VisualElement();
            graphContainer.name = "graph-container";
            graphContainer.style.flexGrow = 1;

            graphPanel.Add(graphContainer);

            mainContent.Add(graphPanel);

            if (mainGraph != null)
            {
                //If there are no nested graphs, then display the main graph.
                if (nestedGraphs.Count == 0)
                    graphContainer.Add(mainGraph);

                toolbarBreadcrumbs.Clear();
                toolbarBreadcrumbs.PushItem(mainGraph.name, () =>
                {
                    MoveToMainGraph();
                });

                for(int i = 0; i < nestedGraphs.Count; i++)
                {
                    var nestedGraph = nestedGraphs[i];

                    int nestedGraphIndex = i;
                    toolbarBreadcrumbs.PushItem(nestedGraph.name, () =>
                    {
                        MoveToNestedGraph(nestedGraphIndex);
                    });

                    //Display the last nested graph.
                    if(i == nestedGraphs.Count - 1)
                    {
                        SetActiveGraphView(nestedGraph);
                        graphContainer.Add(nestedGraph);
                    }
                }

                MoveActiveBlackboardsToGraph(mainGraph);
            }

            if(InspectorPlacement != PanelPlacement.None)
            {
                if (InspectorType == PanelType.SplitView)
                    CreateInspectorSidePanel();
                else if (InspectorType == PanelType.Blackboard)
                    CreateInspectorBlackboard();
            }

            OnCreateGUI();
        }

        protected void SetContainerObject(TObject containerObject)
        {
            string path = AssetDatabase.GetAssetPath(containerObject);

            this.containerObject = containerObject;
            if(!string.IsNullOrEmpty(path))
                EditorPrefs.SetString(ContainerObjectPathPrefsKey, path);
            else
                EditorPrefs.DeleteKey(ContainerObjectPathPrefsKey);

            RefreshContainerObject();

            string iconPath = EditorGUIUtility.isProSkin ? DarkModeIconPath : LightModeIconPath;
            Texture icon = !string.IsNullOrEmpty(iconPath) ? EditorGUIUtility.Load(iconPath) as Texture : null;

            titleContent = new GUIContent(containerObject != null ? containerObject.name : DefaultTitle, icon);
        }
        /// <summary>
        /// This could be called the first time the window was opened, or after a recompile (because the window content will have been destroyed).
        /// </summary>
        protected void RefreshContainerObject()
        {
            containerSerializedObject = new SerializedObject(containerObject);

            OnContainerObjectRefresh();
        }
        private void CreateInspectorSidePanel()
        {
            ispectorSplitView = new SplitView(1, 300, TwoPaneSplitViewOrientation.Horizontal, this.GetType().ToString());
            ispectorSplitView.name = "inspector-split-view";

            VisualElement inspectorContainer = new VisualElement();
            inspectorContainer.name = "inspector-container";
            inspectorContainer.style.backgroundColor = new Color(0.19f, 0.19f, 0.19f, 1f);

            Label inspectorHeader = new Label("Inspector");
            inspectorHeader.style.height = 37;
            inspectorHeader.style.marginBottom = 5;
            inspectorHeader.style.paddingTop = 5;
            inspectorHeader.style.paddingBottom = 5;
            inspectorHeader.style.fontSize = 17;
            inspectorHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            inspectorHeader.style.unityTextAlign = TextAnchor.MiddleCenter;
            inspectorHeader.style.backgroundColor = new Color(0.2235294f, 0.2235294f, 0.2235294f, 1f);
            inspectorHeader.style.borderBottomColor = new Color(0, 0, 0, 0.35f);
            inspectorHeader.style.borderBottomWidth = 1;

            inspectorContainer.Add(inspectorHeader);

            inspectorCollapseButton = new Button(() =>
            {
                isInspectorSplitViewCollapsed = !isInspectorSplitViewCollapsed;

                if(isInspectorSplitViewCollapsed)
                {
                    ispectorSplitView.CollapseChild(1);
                    inspectorCollapseButton.text = "<";
                }
                else
                {
                    ispectorSplitView.UnCollapse();
                    inspectorCollapseButton.text = ">";
                }
            });
            inspectorCollapseButton.RemoveFromClassList("unity-button");
            inspectorCollapseButton.text = ">";
            inspectorCollapseButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            inspectorCollapseButton.style.unityTextAlign = TextAnchor.MiddleCenter;
            inspectorCollapseButton.style.fontSize = 17;
            inspectorCollapseButton.style.position = Position.Absolute;
            inspectorCollapseButton.style.width = 30;
            inspectorCollapseButton.style.height = 37;
            inspectorCollapseButton.style.marginTop = 0;
            inspectorCollapseButton.style.borderTopLeftRadius = 0;
            inspectorCollapseButton.style.borderBottomLeftRadius = 7;
            inspectorCollapseButton.style.borderTopRightRadius = 0;
            inspectorCollapseButton.style.borderBottomRightRadius = 0;
            inspectorCollapseButton.style.borderTopColor = 
            inspectorCollapseButton.style.borderBottomColor = 
            inspectorCollapseButton.style.borderRightColor = 
            inspectorCollapseButton.style.borderLeftColor = new Color(0, 0, 0, 0.4f);
            inspectorCollapseButton.style.borderLeftWidth = 1;
            inspectorCollapseButton.style.borderBottomWidth = 1;
            inspectorCollapseButton.style.alignSelf = Align.FlexEnd;
            inspectorCollapseButton.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 1f);

            graphPanel.Add(inspectorCollapseButton);

            organizeNodesButton = new Button(() =>
            {
                if(activeGraph != null && activeGraph is TafraGraphView<TObject> activeTafraGraph)
                    activeTafraGraph.OrganizeNodes();
            });
            organizeNodesButton.text = "Organize";
            organizeNodesButton.style.position = Position.Absolute;
            organizeNodesButton.style.alignSelf = Align.FlexEnd;
            organizeNodesButton.style.height = 30f;
            organizeNodesButton.style.width = 115f;
            organizeNodesButton.style.bottom = 3;
            organizeNodesButton.style.right = 3;

            graphPanel.Add(organizeNodesButton);

            inspectorView = new InspectorView();

            inspectorContainer.Add(inspectorView);

            ispectorSplitView.Add(graphPanel);
            ispectorSplitView.Add(inspectorContainer);

            mainContent.Add(ispectorSplitView);
        }
        private void CreateInspectorBlackboard()
        {
            inspectorBoardView = new Blackboard(activeGraph)
            {
                title = "Inspector",
                subTitle = ""
            };
            inspectorBoardView.Q<Button>("addButton").style.display = DisplayStyle.None;
            inspectorBoardView.Q<VisualElement>("header").style.minHeight = 52;
            inspectorBoardView.Q<Blackboard>().style.backgroundColor = new StyleColor(new Color(0.19f, 0.19f, 0.19f, 1));

            inspectorView = new InspectorView();

            inspectorBoardView.contentContainer.Add(inspectorView);

            if(activeGraph != null)
                SetInspectorInitialPosition();

            DisplayBlackboard(inspectorBoardView);
        }
        protected void SetInspectorInitialPosition()
        {
            Vector2 inspectorBoardSize = new Vector2(300, 700);
            Vector2 cords = activeGraph.contentContainer.WorldToLocal(new Vector2(this.position.width - (inspectorBoardSize.x) - 7, 7));
            inspectorBoardView.SetPosition(new Rect(cords, inspectorBoardSize));

            inspectorBoardPositionSet = true;
        }

        /// <summary>
        /// Gets fired in the built-in CreateGUI function. You can create and display elements you want to add to the window here. Blackboards for example.
        /// </summary>
        protected virtual void OnCreateGUI() { }
        protected virtual void OnActiveGraphChanged(GraphView newActiveGraphView,  GraphView oldActiveGraphView) { }
        protected virtual void OnSelectionChange()
        {
            if(Selection.activeObject is TObject selectedObject && selectedObject != containerObject)
                SetContainerObject(selectedObject);

            if(containerObject == null && graphContainer != null)
            {
                graphContainer.Clear();
            }
        }
        protected abstract void OnContainerObjectRefresh();
        protected virtual void OnExposablePropertySelected(SerializedProperty property, string title)
        {
            inspectorView.InspectProperty(property, title);
        }
        protected virtual void OnExposablePropertyUnselected(SerializedProperty property)
        {
            //Ignore deselection if the inspector is a blackboard since it deselects element if the user pressed on the blackboard itself which is annoying.
            if(InspectorType == PanelType.Blackboard)
                return;

            inspectorView.UninspectProperty(property);
        }
        protected virtual void OnElementRemoved()
        {
            inspectorView.ForceUninspect();
        }
        protected virtual void OnOpenNestedViewRequest(TafraGraphView<TObject> view)
        {
            DisplayNewNestedGraph(view);
        }

        private void SetActiveGraphView(GraphView newActiveGraphView) 
        {
            GraphView oldActiveGraphView = activeGraph;

            activeGraph = newActiveGraphView;

            if(oldActiveGraphView != null && oldActiveGraphView is TafraGraphView<TObject> oldBrainGraphView)
            {
                oldBrainGraphView.OnExposablePropertySelected = null;
                oldBrainGraphView.OnExposablePropertyUnselected = null;
                oldBrainGraphView.OnOpenNestedViewRequest = null;
                oldBrainGraphView.OnElementRemoved = null;
            }
            if(newActiveGraphView != null && newActiveGraphView is TafraGraphView<TObject> newBrainGraphView)
            {
                newBrainGraphView.OnExposablePropertySelected = OnExposablePropertySelected;
                newBrainGraphView.OnExposablePropertyUnselected = OnExposablePropertyUnselected;
                newBrainGraphView.OnOpenNestedViewRequest = OnOpenNestedViewRequest;
                newBrainGraphView.OnElementRemoved = OnElementRemoved;

                if(!inspectorBoardPositionSet && inspectorBoardView != null)
                    SetInspectorInitialPosition();
            }

            //Remove whatever is displayed in the inspector whenever we change graph.
            if(inspectorView != null)
                inspectorView.ForceUninspect();

            OnActiveGraphChanged(newActiveGraphView, oldActiveGraphView);
        }
        private void MoveActiveBlackboardsToGraph(GraphView targetGraphView)
        {
            for(int i = 0; i < activeBlackboards.Count; i++)
            {
                var blackboard = activeBlackboards[i];

                blackboard.graphView = targetGraphView;
                targetGraphView.Add(blackboard);
            }

        }
        private void MoveToMainGraph()
        {
            DisplayGraph(mainGraph);
        }
        private void MoveToNestedGraph(int nestedGraphIndex)
        { 
            DisplayExistingNestedGraph(nestedGraphIndex);
        }

        /// <summary>
        /// Displays the assigned graph if the GUI was created. Otherwise it will be displayed once it is.
        /// </summary>
        /// <param name="newGraph"></param>
        /// <param name="keepBlackboards"></param>
        public void DisplayGraph(GraphView newGraph, bool keepBlackboards = true)
        {
            if(keepBlackboards)
                MoveActiveBlackboardsToGraph(newGraph);

            //If the GUI was created (CreateGUI got called) this means the graph container was created and we should clear it's content in case previous graphs where there.
            if(graphContainer != null)
            {
                graphContainer.Clear();
                graphContainer.Add(newGraph);
                toolbarBreadcrumbs.Clear();
                toolbarBreadcrumbs.PushItem(newGraph.name, () => 
                {
                    MoveToMainGraph();
                });

                //Pop all the elements in the breadcrumbs except the first one since we're going back to the main graph.
                for(int i = 0; i < toolbarBreadcrumbs.childCount - 1; i++)
                {
                    toolbarBreadcrumbs.PopItem();
                }
            }

            mainGraph = newGraph;
            SetActiveGraphView(newGraph);

            nestedGraphs.Clear();
        }
        /// <summary>
        /// Displays the assigned graph as a nested graph to the current main one if the GUI was created. Otherwise it will be displayed once it is.
        /// </summary>
        /// <param name="newGraph"></param>
        /// <param name="keepBlackboards"></param>
        public void DisplayNewNestedGraph(GraphView newGraph)
        {
            nestedGraphs.Add(newGraph);

            //Move existing blackboards to the new graph.
            MoveActiveBlackboardsToGraph(newGraph);

            //If the GUI was created (CreateGUI got called) this means the graph container was created and we should clear it's content in case previous graphs where there.
            if(graphContainer != null)
            {
                graphContainer.Clear();
                graphContainer.Add(newGraph);
                int nestedGraphIndex = nestedGraphs.Count - 1;
                toolbarBreadcrumbs.PushItem(newGraph.name, () =>
                {
                    MoveToNestedGraph(nestedGraphIndex);
                });
            }

            SetActiveGraphView(newGraph);
        }
        public void DisplayExistingNestedGraph(int graphIndex)
        {
            //If this is the last nested graph, then it's already displayed, no need to display it agian.
            if(graphIndex == nestedGraphs.Count - 1)
                return;

            GraphView newGraph = nestedGraphs[graphIndex];

            //Move existing blackboards to the new graph.
            MoveActiveBlackboardsToGraph(newGraph);

            //If the GUI was created (CreateGUI got called) this means the graph container was created and we should clear it's content in case previous graphs where there.
            if(graphContainer != null)
            {
                graphContainer.Clear();
                graphContainer.Add(newGraph);
            }

            int nestedGraphsToDiscard = nestedGraphs.Count - (graphIndex + 1);

            while(nestedGraphsToDiscard > 0)
            {
                if(graphContainer != null)
                    toolbarBreadcrumbs.PopItem();

                nestedGraphs.RemoveAt(nestedGraphs.Count -1);

                nestedGraphsToDiscard--;
            }

            SetActiveGraphView(newGraph);
        }

        public void DisplayBlackboard(Blackboard blackboard)
        {
            activeBlackboards.Add(blackboard);

            if(mainGraph != null)
            { 
                blackboard.graphView = mainGraph;
                mainGraph.Add(blackboard);
            }
        }
        public void RemoveBlackboard(Blackboard blackboard)
        {
            int blackboardIndex = activeBlackboards.IndexOf(blackboard);

            if(blackboardIndex != -1)
            {
                activeBlackboards.Remove(blackboard);

                if (mainGraph != null)
                    mainGraph.Remove(blackboard);
            }
        }
    }
}