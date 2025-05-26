using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TafraKit.AI3;

namespace TafraKitEditor.AI3
{
    public class LayerButton : Button
    {
        private TextField nameChangeField;
        private bool isSelected = false;
        private string myLayerName;
        private int myLayerIndex;
        
        public LayerButton(string layerName, int layerIndex, Brain brain, Action<int> clickEvent, Action<int> moveUp, Action<int> moveDown, Action<int> deleteRequest, Action<int, string, string> renameRequest)
        {
            myLayerName = layerName;
            text = layerName;
            myLayerIndex = layerIndex;
          
            this.style.height = 35f;

            CreateNameChangeField(renameRequest);

            AddToClassList("layerButton");

            this.clicked += () =>
            {
                clickEvent?.Invoke(myLayerIndex);
            };

            this.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                bool isFirstLayer = myLayerIndex == 0;
                bool isLastLayer = myLayerIndex == brain.Layers.Count - 1;

                DropdownMenuAction.Status moveDownStatus = DropdownMenuAction.Status.Normal;
                DropdownMenuAction.Status deleteStatus = DropdownMenuAction.Status.Normal;
                if(isLastLayer || brain.Layers.Count == 0)
                    moveDownStatus = DropdownMenuAction.Status.Disabled;

                if(brain.Layers.Count == 0)
                    deleteStatus = DropdownMenuAction.Status.Disabled;

                evt.menu.AppendAction("Rename", (x) => 
                {
                    nameChangeField.style.display = DisplayStyle.Flex;
                    nameChangeField.SetValueWithoutNotify(myLayerName);
                    nameChangeField.Focus();
                    this.text = "";
                });
                evt.menu.AppendAction("Move Up", (x) => 
                {
                    moveUp?.Invoke(myLayerIndex);
                }, isFirstLayer ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
                evt.menu.AppendAction("Move Down", (x) => 
                {
                    moveDown?.Invoke(myLayerIndex);
                }, moveDownStatus);
                evt.menu.AppendAction("Delete", (x) => 
                {
                    deleteRequest?.Invoke(myLayerIndex);
                }, deleteStatus);
            }));
        }

        public void ToggleSelectedState(bool isSelected)
        {
            if (this.isSelected == isSelected)
                return;

            this.isSelected = isSelected;
            
            if (isSelected)
                AddToClassList("layerButtonSelected");
            else
                RemoveFromClassList("layerButtonSelected");
        }
        public void SetLayerIndex(int layerIndex)
        {
            myLayerIndex = layerIndex;
        }
        public void SetName(string newName)
        {
            myLayerName = newName;
            text = newName;
            nameChangeField.SetValueWithoutNotify(myLayerName);
        }

        private void CreateNameChangeField(Action<int, string, string> renameRequest)
        {
            nameChangeField = new TextField(myLayerName);
            nameChangeField.Q<Label>().style.display = DisplayStyle.None;
            nameChangeField.style.display = DisplayStyle.None;
            nameChangeField.style.flexGrow = 1;
            nameChangeField.isDelayed = true;

            nameChangeField.RegisterCallback<FocusOutEvent>((e) =>
            {
                text = myLayerName;
                nameChangeField.style.display = DisplayStyle.None;
            });
            nameChangeField.RegisterCallback<KeyDownEvent>(e =>
            {
                if(e.keyCode == KeyCode.Escape)
                {
                    text = myLayerName;
                    nameChangeField.style.display = DisplayStyle.None;
                }
            });
            nameChangeField.RegisterCallback<ChangeEvent<string>>((ev) =>
            {
                renameRequest?.Invoke(myLayerIndex, ev.previousValue, ev.newValue);
                nameChangeField.style.display = DisplayStyle.None;
            });

            Add(nameChangeField);
        }
    }
}