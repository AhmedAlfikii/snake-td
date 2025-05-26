using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEngine;
using UnityEngine.UIElements;
using TafraKitEditor.GraphViews;
using UnityEditor;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(SubstateMachineState))]
    public class SubstateMachineStateNodeEditor : StateNodeEditor
    {
        protected override bool HasInputPort => true;
        protected override bool HasOutputPort => true;

        public SubstateMachineStateNodeEditor(State state) : base(state)
        {
            AddToClassList("subStateMachineStateNode");
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction("Convert to External State Machine", (a) =>
            {
                bool approved = EditorUtility.DisplayDialog("Converting", "The internal state machine will be cleared and moved to an external one, are you sure you want to proceed?", "OK", "Cancel");

                if (!approved)
                    return;

                SubstateMachineState state = node as SubstateMachineState;

                string startingPath = "Assets";

                if (state.HoldingObject != null)
                {
                    startingPath = AssetDatabase.GetAssetPath(state.HoldingObject.GetInstanceID());
                    startingPath = startingPath.Substring(0, startingPath.LastIndexOf('/'));
                }

                string savePath = EditorUtility.SaveFilePanelInProject("Save State Machine", node.Name, "asset", "Save your state machine.", startingPath);

                if (!string.IsNullOrEmpty(savePath))
                {
                    InternalStateMachine originalInternalSM = state.InternalStateMachine as InternalStateMachine;

                    SubStateMachine externalSM = ScriptableObject.CreateInstance<SubStateMachine>();
                    InternalStateMachine copiedInternalSM = new InternalStateMachine();

                    bool copied = TafraEditorUtility.Clone(originalInternalSM, copiedInternalSM);

                    externalSM.EditorSetStateMachine(copiedInternalSM);
                    externalSM.EditorRefreshNodesHoldingObject();

                    AssetDatabase.CreateAsset(externalSM, savePath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();

                    copiedInternalSM.EditorFixChildrenReferences();

                    Selection.activeObject = externalSM;

                    if (state.HoldingObject != null)
                        Undo.RecordObject(state.HoldingObject, "Convert to External BT");

                    //Clear internal behaviour tree
                    state.EditorClearInternalStateMachine();

                    state.EditorSetExternalStateMachine(externalSM);

                    if (state.HoldingObject != null)
                        EditorUtility.SetDirty(state.HoldingObject);
                }
            });

            evt.menu.AppendSeparator();
        }

    }
}
