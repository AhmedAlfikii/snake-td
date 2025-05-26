using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKitEditor.GraphViews;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TafraKitEditor.CharacterControls
{
    public class AbilityWindow : GraphWindow<Ability>
    {
        private BlackboardView blackboardView;

        protected override string DefaultTitle => "Ability";
        protected override string DarkModeIconPath => "Tafra Kit/Icons/d_Ability.png";
        protected override string LightModeIconPath => "Tafra Kit/Icons/Ability.png";
        protected override PanelPlacement InspectorPlacement => GraphWindow<Ability>.PanelPlacement.UpperRight;

        #region Opening Window
        public static void Open(Ability ability)
        {
            AbilityWindow window = GetWindow<AbilityWindow>(typeof(SceneView));

            InitializeWindow(window, ability);
        }
        [MenuItem("Tafra Games/Windows/Character Controls/Ability")]
        public static void OpenContextMenu()
        {
            Open(null);
        }
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int index)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceId);

            if(asset.GetType() == typeof(Ability))
            {
                Ability obj = (Ability)asset;

                Open(obj);

                return true;
            }

            return false;
        }
        #endregion

        protected override void OnCreateGUI()
        {
            CreateBlackboard();
        }
        protected override void OnUndoRedoPerformed()
        {
            base.OnUndoRedoPerformed();

            if(containerObject == null)
                return;

            if(blackboardView != null)
                blackboardView.OnUndoRedoPerformed();
        }
        protected override void OnContainerObjectRefresh()
        {
            containerSerializedObject.Update();

            GraphView graph = new AbilityBTGraphView(containerSerializedObject, null, containerObject, containerObject.name, this);

            DisplayGraph(graph);

            if(blackboardView != null)
                blackboardView.AssignBlackboardObject(containerObject.Blackboard, containerSerializedObject, containerSerializedObject.FindProperty("blackboard"));
        }

        private void CreateBlackboard()
        {
            Rect position = new Rect(new Vector2(7, 7), new Vector2(250, 400));

            if(containerObject != null)
                blackboardView = new BlackboardView(containerObject.Blackboard, containerSerializedObject, containerSerializedObject.FindProperty("blackboard"), activeGraph, "Blackboard", "Properties", position);
            else
                blackboardView = new BlackboardView("Blackboard", "Properties", position);

            blackboardView.OnFieldUnselected = OnExposablePropertyUnselected;
            blackboardView.OnFieldSelected = OnExposablePropertySelected;

            DisplayBlackboard(blackboardView);
        }
    }
}