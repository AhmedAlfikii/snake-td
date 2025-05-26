using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using TafraKit;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(PropertyDependentFieldAttribute))]
    public class PropertyDependentFieldAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            PropertyDependentFieldAttribute att = attribute as PropertyDependentFieldAttribute;
            
            SerializedProperty targetProperty = property.GetSibilingProperty(att.TargetPropertyPath);

            PropertyField field = new PropertyField(property);

            if(targetProperty != null)
            {
                field.schedule.Execute(() =>
                {
                    field.style.display = ShouldBeVisible(targetProperty, att.TargetPropertyValue) ? DisplayStyle.Flex : DisplayStyle.None;
                }).Every(100);
            }

            
            return field;
        }

        public bool ShouldBeVisible(SerializedProperty targetProperty, object targetPropertyValue)
        {
            switch (targetProperty.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return targetProperty.boolValue == (bool)targetPropertyValue;
                case SerializedPropertyType.Integer:
                    return targetProperty.intValue == (int)targetPropertyValue;
                case SerializedPropertyType.Float:
                    return targetProperty.floatValue == (float)targetPropertyValue;
                case SerializedPropertyType.String:
                    return targetProperty.stringValue == (string)targetPropertyValue;
                case SerializedPropertyType.ObjectReference:
                    return targetProperty.objectReferenceValue.ToString() == (string)targetPropertyValue;
                case SerializedPropertyType.Enum:
                    return targetProperty.enumValueIndex == (int)targetPropertyValue;
                case SerializedPropertyType.Character:
                    return Convert.ToChar(targetProperty.intValue) == (char)targetPropertyValue;
                default:
                    return false;
            }
        }
    }
}