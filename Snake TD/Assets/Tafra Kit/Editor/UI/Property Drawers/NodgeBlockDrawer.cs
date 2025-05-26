using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using TafraKit.UI;

namespace TafraKitEditor.UI
{
    [CustomPropertyDrawer(typeof(NodgeBlock), true)]
    public class NodgeBlockDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            Rect drawRect = rect;
            drawRect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty normalNodge = prop.FindPropertyRelative("normalNodge");
            SerializedProperty highlightedNodge = prop.FindPropertyRelative("highlightedNodge");
            SerializedProperty pressedNodge = prop.FindPropertyRelative("pressedNodge");
            SerializedProperty selectedNodge = prop.FindPropertyRelative("selectedNodge");
            SerializedProperty disabledNodge = prop.FindPropertyRelative("disabledNodge");
            SerializedProperty defaultNodgeDuration = prop.FindPropertyRelative("defaultNodgeDuration");
            SerializedProperty defaultNodgeEasing = prop.FindPropertyRelative("defaultNodgeEasing");
            SerializedProperty customPressedNodgeProperties = prop.FindPropertyRelative("customPressedNodgeProperties");
            SerializedProperty pressedNodgeDuration = prop.FindPropertyRelative("pressedNodgeDuration");
            SerializedProperty pressedNodgeEasing = prop.FindPropertyRelative("pressedNodgeEasing");

            EditorGUI.PropertyField(drawRect, normalNodge);

            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, highlightedNodge);

            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, pressedNodge);

            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, selectedNodge);

            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, disabledNodge);

            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, defaultNodgeDuration);

            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            drawRect.height = EditorGUI.GetPropertyHeight(defaultNodgeEasing);
            EditorGUI.PropertyField(drawRect, defaultNodgeEasing);

            drawRect.y += drawRect.height + EditorGUIUtility.standardVerticalSpacing;
            drawRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(drawRect, customPressedNodgeProperties);

            if (customPressedNodgeProperties.boolValue)
            {
                drawRect.y += drawRect.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(drawRect, pressedNodgeDuration);

                drawRect.y += drawRect.height + EditorGUIUtility.standardVerticalSpacing;
                drawRect.height = EditorGUI.GetPropertyHeight(pressedNodgeEasing);
                EditorGUI.PropertyField(drawRect, pressedNodgeEasing);
            }
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            SerializedProperty defaultNodgeEasing = prop.FindPropertyRelative("defaultNodgeEasing");
            SerializedProperty customPressedNodgeProperties = prop.FindPropertyRelative("customPressedNodgeProperties");
            SerializedProperty pressedNodgeEasing = prop.FindPropertyRelative("pressedNodgeEasing");
            int oneLinePropertiesCount = 7;
            float totalHeight = 0;

            if (customPressedNodgeProperties.boolValue)
            {
                oneLinePropertiesCount++;
                totalHeight += EditorGUI.GetPropertyHeight(pressedNodgeEasing);
            }

            totalHeight += oneLinePropertiesCount* EditorGUIUtility.singleLineHeight + (oneLinePropertiesCount - 1) * EditorGUIUtility.standardVerticalSpacing
                + EditorGUI.GetPropertyHeight(defaultNodgeEasing);

            return totalHeight;
        }
    }
}