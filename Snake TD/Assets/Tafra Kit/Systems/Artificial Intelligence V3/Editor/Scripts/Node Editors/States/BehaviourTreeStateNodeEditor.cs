using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(BehaviourTreeState))]
    public class BehaviourTreeStateNodeEditor : StateNodeEditor
    {
        protected override bool HasInputPort => true;
        protected override bool HasOutputPort => true;

        public BehaviourTreeStateNodeEditor(State state) : base(state)
        {
            AddToClassList("behaviourTreeStateNode");
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction("Convert to External Tree", (a) =>
            {
                bool approved = EditorUtility.DisplayDialog("Converting", "The internal behaviour tree will be cleared and moved to an external one, are you sure you want to proceed?", "OK", "Cancel");

                if (!approved)
                    return;

                BehaviourTreeState state = node as BehaviourTreeState;
               
                string startingPath = "Assets";

                if (state.HoldingObject != null)
                {
                    startingPath = AssetDatabase.GetAssetPath(state.HoldingObject.GetInstanceID());
                    startingPath = startingPath.Substring(0, startingPath.LastIndexOf('/'));
                }

                string savePath = EditorUtility.SaveFilePanelInProject("Save Behaviour Tree", node.Name, "asset", "Save your behaviour tree.", startingPath);

                if (!string.IsNullOrEmpty(savePath))
                {
                    InternalBehaviourTree originalInternalBT = state.InternalBehaviourTree as InternalBehaviourTree;

                    BehaviourTree externalBT = ScriptableObject.CreateInstance<BehaviourTree>();
                    InternalBehaviourTree copiedInternalBT = new InternalBehaviourTree();
                    
                    bool copied = TafraEditorUtility.Clone(originalInternalBT, copiedInternalBT);

                    copiedInternalBT.EditorFixChildrenReferences();

                    externalBT.EditorSetTree(copiedInternalBT);
                    externalBT.EditorRefreshNodesHoldingObject();

                    AssetDatabase.CreateAsset(externalBT, savePath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = externalBT;

                    if (state.HoldingObject != null)
                        Undo.RecordObject(state.HoldingObject, "Convert to External BT");

                    //Clear internal behaviour tree
                    state.EditorClearInternalBehaviourTree();

                    state.EditorSetExternalBehaviourTree(externalBT);
                    
                    if (state.HoldingObject != null)
                        EditorUtility.SetDirty(state.HoldingObject);
                }
            });

            evt.menu.AppendSeparator();
        }
    }
}
