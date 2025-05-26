using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using TafraKitEditor.GraphViews;
using System;
using Object = UnityEngine.Object;

namespace TafraKitEditor.AI3
{
    public class BrainWindow : GraphWindow<Brain>
    {
        private BlackboardView blackboardView;
        private LayersBoardView layersBoardView;
        private bool layersBoardPositionSet;
        private int lastDisplayedLayerIndex;

        protected override string DefaultTitle => "Brain";
        protected override string DarkModeIconPath => "Tafra Kit/Icons/d_Brain.png";
        protected override string LightModeIconPath => "Tafra Kit/Icons/Brain.png";
        protected override PanelPlacement InspectorPlacement => PanelPlacement.UpperRight;
        protected override PanelType InspectorType => PanelType.SplitView;

        #region Opening Window
        public static void Open(Brain brain)
        {
            BrainWindow window = GetWindow<BrainWindow>(typeof(SceneView));

            InitializeWindow(window, brain);
        }
        [MenuItem("Tafra Games/Windows/AI/Brain")]
        public static void OpenContextMenu()
        {
            Open(null);
        }
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int index)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceId);

            if(asset.GetType() == typeof(Brain))
            {
                Brain obj = (Brain)asset;

                Open(obj);

                return true;
            }

            return false;
        }
        #endregion

        protected override void OnUndoRedoPerformed()
        {
            if (containerObject == null) 
                return;

            if (containerObject.DisplayedLayerIndex != lastDisplayedLayerIndex)
                ShowBrainLayer(containerObject.DisplayedLayerIndex);

            if(activeGraph != null && activeGraph is BrainGraphView activeBrainGraph)
            {
                //Refresh the state machine graph's layer in case it was reordered.
                //if(activeBrainGraph is StateMachineGraphView stateMachineGraphView)
                //    stateMachineGraphView.AssignStateMachine(DisplayedLayer.StateMachine);

                activeBrainGraph.OnUndoRedoPerformed();
            }

            if (blackboardView != null)
                blackboardView.OnUndoRedoPerformed();

            if(layersBoardView != null)
                layersBoardView.OnUndoRedoPerformed();
        }
        protected override void OnCreateGUI()
        {
            CreateBlackboard();
            CreateLayersBoard(); 
        }
        protected override void OnSelectionChange()
        {
            if(Selection.activeObject is GameObject selectedGO && selectedGO.TryGetComponent(out AIAgent agent) && agent.Brain != null && agent.Brain != containerObject)
                SetContainerObject(agent.Brain);

            base.OnSelectionChange();
        }
        protected override void OnActiveGraphChanged(GraphView newActiveGraphView, GraphView oldActiveGraphView)
        {
            if(oldActiveGraphView != null && oldActiveGraphView is BrainGraphView oldBrainGraphView)
            {
                oldBrainGraphView.OnExposablePropertySelected = null;
                oldBrainGraphView.OnExposablePropertyUnselected = null;
                oldBrainGraphView.OnOpenNestedViewRequest = null;
                oldBrainGraphView.OnElementRemoved = null;
            }
            if(newActiveGraphView != null && newActiveGraphView is BrainGraphView newBrainGraphView)
            {
                newBrainGraphView.OnExposablePropertySelected = OnExposablePropertySelected;
                newBrainGraphView.OnExposablePropertyUnselected = OnExposablePropertyUnselected;
                newBrainGraphView.OnOpenNestedViewRequest = OnOpenNestedViewRequest;
                newBrainGraphView.OnElementRemoved = OnElementRemoved;

                if(!inspectorBoardPositionSet && inspectorBoardView != null)
                    SetInspectorInitialPosition();
      
                if(!layersBoardPositionSet && layersBoardView != null)
                    SetLayersBoardInitialPosition();
            }

            //This doesn't seem to be doing anything. Added it because sometimes blackboard fields error when attempting to delete because the serialized object target is null.
            if(blackboardView != null)
                blackboardView.graphView = newActiveGraphView;
        }
        protected override void OnContainerObjectRefresh()
        {
            //Always open the first layer in a brain once it's opened.
            ShowBrainLayer(0);

            if(blackboardView != null)
                blackboardView.AssignBlackboardObject(containerObject.Blackboard, containerSerializedObject, containerSerializedObject.FindProperty("blackboard"));

            if(layersBoardView != null)
                layersBoardView.AssignBrain(containerObject, containerSerializedObject);
        }
        private void OnLayerButtonPressed(int layerIndex)
        {
            ShowBrainLayer(layerIndex);
        }
        private void OnSelectedLayerReordererd(int newIndex)
        {
            containerObject.DisplayedLayerIndex = newIndex;

            ShowBrainLayer(newIndex);
        }

        //Temporarily added until the brain graphs are turned into tafra graph view.
        private void OnOpenNestedViewRequest(GraphView graphView)
        { 
            DisplayNewNestedGraph(graphView);
        }
        private void ShowBrainLayer(int layerIndex)
        {
            containerSerializedObject.Update();

            lastDisplayedLayerIndex = layerIndex;

            SerializedProperty stateMachineProperty = containerSerializedObject.FindProperty("layers").GetArrayElementAtIndex(layerIndex).FindPropertyRelative("stateMachine");

            CreateAndDisplayStateMachineGraph(containerSerializedObject, stateMachineProperty, containerObject.Layers[layerIndex].StateMachine, containerObject.Layers[layerIndex].Name, true);

            containerObject.DisplayedLayerIndex = layerIndex;
        }
        private void CreateAndDisplayStateMachineGraph(SerializedObject mainSerializedObject, SerializedProperty serializedProperty, IStateMachine stateMachine, string stateMachineName, bool isMainGraph)
        {
            GraphView graph = null;

            if(stateMachine != null)
                graph = new StateMachineGraphView(mainSerializedObject, serializedProperty, stateMachine, stateMachineName, this);

            if(graph != null)
            {
                if (isMainGraph)
                    DisplayGraph(graph);
                else
                    DisplayNewNestedGraph(graph);
            }
        }
        private void CreateBlackboard()
        {
            Rect position = new Rect(new Vector2(7, 7), new Vector2(250, 400));

            if (containerObject != null)
                blackboardView = new BlackboardView(containerObject.Blackboard, containerSerializedObject, containerSerializedObject.FindProperty("blackboard"), activeGraph, "Blackboard", "Properties", position);
            else
                blackboardView = new BlackboardView("Blackboard", "Properties", position);
            
            blackboardView.OnFieldUnselected = OnExposablePropertyUnselected;
            blackboardView.OnFieldSelected = OnExposablePropertySelected;

            DisplayBlackboard(blackboardView);
        }
        private void CreateLayersBoard()
        {
            if (containerObject != null)
                layersBoardView = new LayersBoardView(containerObject, containerSerializedObject, activeGraph, "Layers", "");
            else
                layersBoardView = new LayersBoardView("Layers", "");

            layersBoardView.OnLayerButtonPressed = OnLayerButtonPressed;
            layersBoardView.OnSelectedLayerReordererd = OnSelectedLayerReordererd;

            if(activeGraph != null)
                SetLayersBoardInitialPosition();

            DisplayBlackboard(layersBoardView);
        }

        private void SetLayersBoardInitialPosition()
        {
            Vector2 layersBoardSize = new Vector2(175, 250);
            Vector2 cords = activeGraph.contentContainer.WorldToLocal(new Vector2(7, this.position.height - (layersBoardSize.y) - 30));
            layersBoardView.SetPosition(new Rect(cords, layersBoardSize));

            layersBoardPositionSet = true;
        }
    }
}