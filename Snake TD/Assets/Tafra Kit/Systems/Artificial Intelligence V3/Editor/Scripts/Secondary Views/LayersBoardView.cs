using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using TafraKit.AI3;
using TafraKit.Internal.AI3;

namespace TafraKitEditor.AI3
{
    public class LayersBoardView : Blackboard
    {
        public Action<int> OnLayerButtonPressed;
        public Action<int> OnSelectedLayerReordererd;

        private Brain brain;
        private SerializedObject mainSerializedObject;
        private ScrollView layersContainer;
        private List<LayerButton> layerButtons = new List<LayerButton>();
        private int selectedLayerIndex = -1;

        public LayersBoardView(Brain brain, SerializedObject mainSerializedObject, GraphView graphView, string title, string subtitle) : base(graphView)
        {
            Initialize(title, subtitle);

            AssignBrain(brain, mainSerializedObject);
        }
        public LayersBoardView(string title, string subTitle)
        {
            Initialize(title, subTitle);
        }

        private void Initialize(string title, string subTitle)
        {
            this.title = title;
            this.subTitle = subTitle;

            this.Q<VisualElement>("header").style.minHeight = 52;

            layersContainer = new ScrollView();
            layersContainer.style.flexGrow = 1;
            contentContainer.Add(layersContainer);

            addItemRequested = OnAddItemRequest;
        }
        
        public void OnUndoRedoPerformed()
        {
            if (brain == null)
                return;

            PopulateBoard();
            RefreshSelectedButton();
        }
        private void OnAddItemRequest(Blackboard blackboard)
        {
            if (brain == null)
                return;

            Undo.RecordObject(mainSerializedObject.targetObject, "Create Layer");

            Layer newLayer = brain.CreateLayer();

            AddLayerButton(newLayer, brain.Layers.Count - 1);

            EditorUtility.SetDirty(mainSerializedObject.targetObject);
        }

        public void AssignBrain(Brain brain, SerializedObject mainSerializedObject)
        {
            selectedLayerIndex = brain.DisplayedLayerIndex;

            this.brain = brain;
            this.mainSerializedObject = mainSerializedObject;

            PopulateBoard();
            UpdateButtonsSelectedState();
        }
        
        private void PopulateBoard()
        {
            layerButtons.Clear();
            layersContainer.Clear();

            List<Layer> layers = brain.Layers;
            int layersCount = layers.Count;

            for (int i = 0; i < layersCount; i++)
            {
                Layer layer = layers[i];

                AddLayerButton(layer, i);
            }
        }
        private void AddLayerButton(Layer layer, int layerIndex)
        {
            LayerButton layerButton = new LayerButton(layer.Name, layerIndex, brain, SelectLayerButton, MoveLayerUp, MoveLayerDown, OnLayerButtonDeleteRequest, OnLayerRenameRequest);

            layersContainer.Add(layerButton);
            layerButtons.Add(layerButton);
        }

        private void SelectLayerButton(int layerIndex)
        {
            if (layerIndex == selectedLayerIndex)
                return;

            selectedLayerIndex = layerIndex;

            UpdateButtonsSelectedState();

            OnLayerButtonPressed?.Invoke(layerIndex);
        }
        public void UpdateButtonsSelectedState()
        {
            for (int i = 0; i < layerButtons.Count; i++)
            {
                var button = layerButtons[i];

                button.ToggleSelectedState(i == selectedLayerIndex);
            }
        }
        private void MoveLayerUp(int layerIndex)
        {
            if(layerIndex == 0)
                return;

            Undo.RecordObject(mainSerializedObject.targetObject, "Move Layer Up");
         
            Layer tempLayer = brain.Layers[layerIndex];
           
            int newIndex = layerIndex - 1;

            brain.Layers[layerIndex] = brain.Layers[newIndex];
            brain.Layers[newIndex] = tempLayer;

            EditorUtility.SetDirty(mainSerializedObject.targetObject);

            bool selectedLayerIndexChanged = false;
            if(selectedLayerIndex == layerIndex)
            {
                selectedLayerIndex = newIndex;
                selectedLayerIndexChanged = true;
            }
            else if(selectedLayerIndex == newIndex)
            {
                selectedLayerIndex = layerIndex;
                selectedLayerIndexChanged = true;
            }

            if (selectedLayerIndexChanged)
                OnSelectedLayerReordererd?.Invoke(selectedLayerIndex);

            PopulateBoard();
            UpdateButtonsSelectedState();
        }
        private void MoveLayerDown(int layerIndex)
        {
            if(layerIndex == brain.Layers.Count - 1)
                return;

            Undo.RecordObject(mainSerializedObject.targetObject, "Move Layer Down");

            Layer tempLayer = brain.Layers[layerIndex];

            int newIndex = layerIndex + 1;
         
            brain.Layers[layerIndex] = brain.Layers[newIndex];
            brain.Layers[newIndex] = tempLayer;

            EditorUtility.SetDirty(mainSerializedObject.targetObject);

            bool selectedLayerIndexChanged = false;
            if(selectedLayerIndex == layerIndex)
            {
                selectedLayerIndex = newIndex;
                selectedLayerIndexChanged = true;
            }
            else if(selectedLayerIndex == newIndex)
            {
                selectedLayerIndex = layerIndex;
                selectedLayerIndexChanged = true;
            }

            if(selectedLayerIndexChanged)
                OnSelectedLayerReordererd?.Invoke(selectedLayerIndex);

            PopulateBoard();
            UpdateButtonsSelectedState();
        }
        private void OnLayerRenameRequest(int layerIndex, string oldName, string newName)
        {
            if(newName.Length < 1)
                return;

            Undo.RecordObject(mainSerializedObject.targetObject, "Delete Layer");

            string finalName = ValidateLayerName(layerIndex, newName, oldName);

            brain.Layers[layerIndex].Name = finalName;
            layerButtons[layerIndex].SetName(finalName);

            EditorUtility.SetDirty(mainSerializedObject.targetObject);
        }
        private void OnLayerButtonDeleteRequest(int layerIndex)
        {
            layersContainer.RemoveAt(layerIndex);
            layerButtons.RemoveAt(layerIndex);

            if(selectedLayerIndex == layerIndex)
                SelectLayerButton(layerIndex - 1);

            for(int i = 0; i < layerButtons.Count; i++)
            {
                layerButtons[i].SetLayerIndex(i);
            }

            Undo.RecordObject(mainSerializedObject.targetObject, "Delete Layer");

            brain.Layers.RemoveAt(layerIndex);

            EditorUtility.SetDirty(mainSerializedObject.targetObject);
        }
        private string ValidateLayerName(int layerIndex, string layerName, string originalName = null)
        {
            layerName = layerName.Trim();

            bool isUnique = false;
            int nameIteration = 0;
            while(!isUnique)
            {
                isUnique = true;

                for(int i = 0; i < brain.Layers.Count; i++)
                {
                    string n = brain.Layers[i].Name;
                    
                    if(i == layerIndex)
                        continue;

                    if(layerName == n)
                    {
                        if(nameIteration == 0)
                            layerName += " (1)";
                        else
                        {
                            layerName = layerName.Remove(layerName.LastIndexOf('('));
                            layerName += $"({nameIteration + 1})";
                        }

                        //If this object's name went back to the original name, then no need to increase its iterations, just use the original name.
                        if(originalName != null && layerName == originalName)
                        {
                            isUnique = true;
                            break;
                        }

                        nameIteration++;
                        isUnique = false;
                    }
                }
            }

            return layerName;
        }

        public void RefreshSelectedButton()
        {
            for(int i = 0; i < layerButtons.Count; i++)
            {
                var button = layerButtons[i];

                button.ToggleSelectedState(i == brain.DisplayedLayerIndex);
            }

            selectedLayerIndex = brain.DisplayedLayerIndex;
        }
    }
}