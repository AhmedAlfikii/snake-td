using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.GraphViews
{
    public class ExternalBlackboardFieldView : VisualElement
    {
        public Action<ExternalBlackboardFieldView, string, string> OnRenameRequest;
        public Action OnDeleted;

        private ExposableProperty exposableProperty;
        private SerializedProperty serializedProperty;
        private Label label;
        private PropertyField nameFieldProperty;
        private TextField nameField;

        public ExposableProperty ExposableProperty => exposableProperty;

        public ExternalBlackboardFieldView(SerializedProperty serializedProperty, ExposableProperty exposableProperty)
        {
            this.exposableProperty = exposableProperty;
            this.serializedProperty = serializedProperty;

            serializedProperty.serializedObject.Update();

            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;

            #region Label
            label = new Label(serializedProperty.FindPropertyRelative("name").stringValue);
            label.style.width = 200;
            label.pickingMode = PickingMode.Position;
            label.focusable = true;
            label.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent ev) =>
            {
                ev.menu.AppendAction("Rename", (a) => {
                    Rename();
                });
                ev.menu.AppendAction("Delete", (a) => {
                    Delete();
                });
            }));
            Add(label);
            #endregion

            #region Name Field
            nameField = new TextField();
            nameField.style.width = 200;
            nameField.style.display = DisplayStyle.None;
            nameField.isDelayed = true;
            nameField.RegisterCallback<FocusOutEvent>((e) =>
            {
                nameField.style.display = DisplayStyle.None;
                label.style.display = DisplayStyle.Flex;
            });
            nameField.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Escape)
                {
                    nameField.style.display = DisplayStyle.None;
                    label.style.display = DisplayStyle.Flex;
                }
            });
            nameField.RegisterCallback<ChangeEvent<string>>((ev) =>
            {
                nameField.style.display = DisplayStyle.None;
                OnRenameRequest?.Invoke(this, ev.previousValue, ev.newValue);
                label.style.display = DisplayStyle.Flex;
            });
            Add(nameField);
            #endregion

            PropertyField valueField = new PropertyField(serializedProperty.FindPropertyRelative("value"), "");
            valueField.style.flexGrow = 1;

            Add(valueField);

            this.Bind(serializedProperty.serializedObject);
        }

        private void Rename()
        {
            nameField.SetValueWithoutNotify(serializedProperty.FindPropertyRelative("name").stringValue);

            label.style.display = DisplayStyle.None;
            nameField.style.display = DisplayStyle.Flex;
            nameField.Focus();
        }
        private void Delete()
        {
            serializedProperty.Parent().DeleteArrayElementAtIndex(serializedProperty.GetMyIndexInParentArray());
            serializedProperty.serializedObject.ApplyModifiedProperties();
            OnDeleted?.Invoke();
        }

        public void FocusName()
        {
            Rename();
        }
        public void SetName(string newName)
        {
            label.text = newName;
            serializedProperty.FindPropertyRelative("name").stringValue = newName;
        }
    }
}