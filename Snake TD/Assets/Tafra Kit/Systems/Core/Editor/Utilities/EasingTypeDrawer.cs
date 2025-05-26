using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(EasingType))]
    public class EasingTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect easingRect2 = new Rect(position.x, position.y, position.width, position.height / 2f);
            Rect parametersRect = new Rect(position.x + 20, position.y + position.height / 2f, position.width - 20, position.height / 2f);

            SerializedProperty easingProp = property.FindPropertyRelative("Easing");
            SerializedProperty parameterProp = null;

            switch (easingProp.enumNames[easingProp.enumValueIndex])
            {
                case "Custom":
                    parameterProp = property.FindPropertyRelative("Parameters").FindPropertyRelative("Custom").FindPropertyRelative("Curve");
                    break;
                case "EaseIn":
                case "EaseOut":
                case "EaseInOut":
                    parameterProp = property.FindPropertyRelative("Parameters").FindPropertyRelative("EaseInOut").FindPropertyRelative("EasingPower");
                    break;
                case "EaseInBack":
                case "EaseOutBack":
                case "EaseInOutBack":
                    parameterProp = property.FindPropertyRelative("Parameters").FindPropertyRelative("EaseInOutBack").FindPropertyRelative("BackPower");
                    break;
                case "EaseInElastic":
                case "EaseOutElastic":
                case "EaseInOutElastic":
                    parameterProp = property.FindPropertyRelative("Parameters").FindPropertyRelative("EaseInOutElastic").FindPropertyRelative("ElasticityPower");
                    break;
            }

            EditorGUI.PropertyField(easingRect2, easingProp, new GUIContent(property.displayName));

            if (parameterProp != null)
                EditorGUI.PropertyField(parametersRect, parameterProp);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int propertiesCount = 1 + (HasAdditionalProp(property) ? 1 : 0);

            return propertiesCount * EditorGUIUtility.singleLineHeight + propertiesCount - 1 * EditorGUIUtility.standardVerticalSpacing;
        }

        private bool HasAdditionalProp(SerializedProperty property)
        {
            SerializedProperty easingProp = property.FindPropertyRelative("Easing");
            return easingProp.enumNames[easingProp.enumValueIndex] != "Linear";
        }
    }
}