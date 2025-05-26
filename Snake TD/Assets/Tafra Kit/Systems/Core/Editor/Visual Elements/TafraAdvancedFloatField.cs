using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using TafraKit;

namespace TafraKitEditor
{
    public class TafraAdvancedFloatField : VisualElement
    {
        private SerializedProperty property;
        private SerializedProperty floatTypeProperty;

        private PropertyBox propertyBox;
        private PropertyField floatTypeField;

        public TafraAdvancedFloatField(SerializedProperty property, string displayName, string tooltip = "")
        {
            this.property = property;

            propertyBox = new PropertyBox(property, displayName, "", tooltip, true);
            Add(propertyBox);

            floatTypeProperty = property.FindPropertyRelative("floatType");
            floatTypeField = new PropertyField(floatTypeProperty, "");
            floatTypeField.style.marginRight = 5;
            propertyBox.Header.Add(floatTypeField);

            this.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachedToPanel);

            DrawContent();
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            floatTypeField.RegisterCallback<SerializedPropertyChangeEvent>(FloatTypeChanged);
        }
        private void OnDetachedToPanel(DetachFromPanelEvent evt)
        {
            floatTypeField.UnregisterCallback<SerializedPropertyChangeEvent>(FloatTypeChanged);
        }
        private void FloatTypeChanged(SerializedPropertyChangeEvent evt)
        {
            floatTypeField.UnregisterCallback<SerializedPropertyChangeEvent>(FloatTypeChanged);

            DrawContent();

            floatTypeField.schedule.Execute(() =>
            {
                floatTypeField.RegisterCallback<SerializedPropertyChangeEvent>(FloatTypeChanged);
            });
        }

        private void DrawContent()
        {
            propertyBox.Content.Clear();

            TafraAdvancedFloat.FloatType selectedFloatType = (TafraAdvancedFloat.FloatType)floatTypeProperty.enumValueIndex;

            switch(selectedFloatType)
            {
                case TafraAdvancedFloat.FloatType.Value:
                    PropertyField valueField = new PropertyField(property.FindPropertyRelative("value"));
                    propertyBox.Content.Add(valueField);
                    break;
                case TafraAdvancedFloat.FloatType.ScriptableObject:
                    PropertyField soField = new PropertyField(property.FindPropertyRelative("scriptableVariableAsset"));
                    propertyBox.Content.Add(soField);
                    break;
                case TafraAdvancedFloat.FloatType.TafraFloatRange:
                    PropertyField minField = new PropertyField(property.FindPropertyRelative("min"));
                    propertyBox.Content.Add(minField);
                   
                    PropertyField maxField = new PropertyField(property.FindPropertyRelative("max"));
                    propertyBox.Content.Add(maxField);
                  
                    PropertyField randomizeSignField = new PropertyField(property.FindPropertyRelative("randomizeSign"));
                    propertyBox.Content.Add(randomizeSignField);
                    break;
                default:
                    break;
            }


            SerializedProperty performOperationProperty = property.FindPropertyRelative("performOperation");

            PropertyField operationValueField = new PropertyField(property.FindPropertyRelative("operationValue"));
            PropertyField operationField = new PropertyField(property.FindPropertyRelative("operation"));
           
            operationValueField.style.display = performOperationProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            operationField.style.display = performOperationProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;

            PropertyField performOperationField = new PropertyField(performOperationProperty);
            performOperationField.RegisterValueChangeCallback((ev) =>
            {
                operationValueField.style.display = ev.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                operationField.style.display = ev.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            propertyBox.Content.Add(performOperationField);
            propertyBox.Content.Add(operationValueField);
            propertyBox.Content.Add(operationField);

            this.Bind(property.serializedObject);
        }
    }
}