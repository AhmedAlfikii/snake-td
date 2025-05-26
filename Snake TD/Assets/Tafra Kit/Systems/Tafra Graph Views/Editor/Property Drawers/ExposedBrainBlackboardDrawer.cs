using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using TafraKit.GraphViews;
using TafraKit;

namespace TafraKitEditor.GraphViews
{
    [CustomPropertyDrawer(typeof(ExposedBrainBlackboard))]
    public class ExposedBrainBlackboardDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            int totalFieldsAdded = PopulateExposedPropertiesView(property, container);

            if(totalFieldsAdded == 0)
            {
                Label noPropsLabel = new Label("No brain properties were exposed.");
                noPropsLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                noPropsLabel.style.marginTop = noPropsLabel.style.marginBottom = 4;
                return noPropsLabel;
            }
            else
                return container;
        }

        private int PopulateExposedPropertiesView(SerializedProperty property, VisualElement container)
        {
            int floatFields = CreateExposedPropertiesListGUI<float>(property.FindPropertyRelative("floatProperties"), container);
            int advancedFloatFields = CreateExposedPropertiesListGUI<TafraAdvancedFloat>(property.FindPropertyRelative("advancedFloatProperties"), container);
            int intFields = CreateExposedPropertiesListGUI<int>(property.FindPropertyRelative("intProperties"), container);
            int boolFields = CreateExposedPropertiesListGUI<bool>(property.FindPropertyRelative("boolProperties"), container);
            int stringFields = CreateExposedPropertiesListGUI<string>(property.FindPropertyRelative("stringProperties"), container);
            int vector3Fields = CreateExposedPropertiesListGUI<Vector3>(property.FindPropertyRelative("vector3Properties"), container);
            int goFields = CreateExposedPropertiesListGUI<GameObject>(property.FindPropertyRelative("gameObjectProperties"), container);
            int soFields = CreateExposedPropertiesListGUI<ScriptableObject>(property.FindPropertyRelative("scriptableObjectProperties"), container);
            int actorFields = CreateExposedPropertiesListGUI<TafraActor>(property.FindPropertyRelative("actorProperties"), container);
            int objFields = CreateExposedPropertiesListGUI<UnityEngine.Object>(property.FindPropertyRelative("objectProperties"), container);

            return floatFields + advancedFloatFields + intFields + boolFields + stringFields + vector3Fields + goFields + soFields + actorFields + objFields;
        }
        private int CreateExposedPropertiesListGUI<T>(SerializedProperty properties, VisualElement container) 
        {
            for(int i = 0; i < properties.arraySize; i++)
            {
                SerializedProperty property = properties.GetArrayElementAtIndex(i);

                SerializedProperty nameProperty = property.FindPropertyRelative("name");
                SerializedProperty tooltipProperty = property.FindPropertyRelative("tooltip");
                SerializedProperty valueProperty = property.FindPropertyRelative("value");

                VisualElement propertyField = CreatePropertyField<T>(nameProperty, tooltipProperty, valueProperty);

                if(propertyField != null)
                {
                    propertyField.style.marginTop = propertyField.style.marginBottom = 2;

                    container.Add(propertyField);
                }
                else
                    container.Add(new Label($"{nameProperty.stringValue}: Type value field wasn't handled. Fix this."));
            }

            return properties.arraySize;
        }
        private VisualElement CreatePropertyField<T>(SerializedProperty nameProperty, SerializedProperty tooltipProperty, SerializedProperty valueProperty)
        {
            Type propertyType = typeof(T);

            string tooltip = tooltipProperty.stringValue;

            if(propertyType == typeof(float))
            {
                FloatField field = new FloatField(nameProperty.stringValue);
                field.tooltip = tooltip;
                field.bindingPath = valueProperty.propertyPath;

                return field;
            }
            else if(propertyType == typeof(TafraAdvancedFloat))
            {
                TafraAdvancedFloatField field = new TafraAdvancedFloatField(valueProperty, nameProperty.stringValue, tooltip);

                return field;
            }
            else if(propertyType == typeof(int))
            {
                IntegerField field = new IntegerField(nameProperty.stringValue);
                field.bindingPath = valueProperty.propertyPath;
                field.tooltip = tooltip;

                return field;
            }
            else if(propertyType == typeof(bool))
            {
                Toggle field = new Toggle(nameProperty.stringValue);
                field.bindingPath = valueProperty.propertyPath;
                field.tooltip = tooltip;

                return field;
            }
            else if(propertyType == typeof(string))
            {
                TextField field = new TextField(nameProperty.stringValue);
                field.bindingPath = valueProperty.propertyPath;
                field.tooltip = tooltip;

                return field;
            }
            else if(propertyType == typeof(Vector3))
            {
                Vector3Field field = new Vector3Field(nameProperty.stringValue);
                field.bindingPath = valueProperty.propertyPath;
                field.tooltip = tooltip;

                return field;
            }
            else if(propertyType == typeof(GameObject) || propertyType == typeof(ScriptableObject) || propertyType == typeof(TafraActor) || propertyType == typeof(UnityEngine.Object))
            {
                ObjectField field = new ObjectField(nameProperty.stringValue);
                field.objectType = propertyType;
                field.bindingPath = valueProperty.propertyPath;
                field.tooltip = tooltip;

                return field;
            }

            return null;
        }
    }
}