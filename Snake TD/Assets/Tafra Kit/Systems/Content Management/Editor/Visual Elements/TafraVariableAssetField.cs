using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TafraKit.ContentManagement;
using TafraKit;

namespace TafraKitEditor
{
    public class TafraVariableAssetField : VisualElement
    {
        private SerializedProperty property;
        private SerializedProperty valueTypeProperty;

        private PropertyBox propertyBox;
        private PropertyField valueTypeField;

        public TafraVariableAssetField(SerializedProperty property, string displayName, string tooltip = "")
        {
            this.property = property;

            propertyBox = new PropertyBox(property, displayName, "", property.tooltip, true);

            valueTypeProperty = property.FindPropertyRelative("valueType");
            valueTypeField = new PropertyField(valueTypeProperty, "");
            valueTypeField.style.marginRight = 5;
            propertyBox.Header.Add(valueTypeField);

            Add(propertyBox);

            this.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachedToPanel);

            DrawContent();
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            valueTypeField.RegisterCallback<SerializedPropertyChangeEvent>(AssetTypeChanged);
        }
        private void OnDetachedToPanel(DetachFromPanelEvent evt)
        {
            valueTypeField.UnregisterCallback<SerializedPropertyChangeEvent>(AssetTypeChanged);
        }
        private void AssetTypeChanged(SerializedPropertyChangeEvent evt)
        {
            valueTypeField.UnregisterCallback<SerializedPropertyChangeEvent>(AssetTypeChanged);

            DrawContent();

            valueTypeField.schedule.Execute(() =>
            {
                valueTypeField.RegisterCallback<SerializedPropertyChangeEvent>(AssetTypeChanged);
            });
        }

        private void DrawContent()
        {
            propertyBox.Content.Clear();

            //VisualElement contentRow = new VisualElement();
            //contentRow.name = "content-row";
            //contentRow.style.flexDirection = FlexDirection.Row;
            //propertyBox.Content.Add(contentRow);

            TafraVariableValueType selectedFloatType = (TafraVariableValueType)valueTypeProperty.enumValueIndex;

            switch (selectedFloatType)
            {
                case TafraVariableValueType.Value:
                    PropertyField valuePF = new PropertyField(property.FindPropertyRelative("value"));
                    valuePF.style.flexGrow = 1;
                    propertyBox.Content.Add(valuePF);
                    break;
                case TafraVariableValueType.ScriptableObject:
                    PropertyField scriptableVariableAssetPF = new PropertyField(property.FindPropertyRelative("scriptableVariableAsset"));
                    scriptableVariableAssetPF.style.flexGrow = 1;
                    propertyBox.Content.Add(scriptableVariableAssetPF);
                    break;
                default:
                    break;
            }


            this.Bind(property.serializedObject);
        }
    }
}