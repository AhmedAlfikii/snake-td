using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(IntRange))]
    public class IntRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //Sbaka for now (NEW: commented)
            //Rect labelRect = position;
            //labelRect.x += 60;
            //EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), label);

            //position = new Rect(position.x + 200, position.y, position.width - 200, position.height);
            //EditorGUI.LabelField(labelRect, label);
            ////////////////////////////////////////

            //Instead of this because the prefix label is wrongly placed
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float portionWidth = position.width / 3;
            float txtWidth = position.width / 5;

            Rect minTXTRect = new Rect(position.x, position.y, txtWidth, position.height);
            Rect minFieldRect = new Rect(position.x + txtWidth, position.y, portionWidth, position.height);
            Rect maxTXTRect = new Rect(position.x + portionWidth + txtWidth, position.y, txtWidth, position.height);
            Rect maxFieldRect = new Rect(position.x + position.width - portionWidth, position.y, portionWidth, position.height);

            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.UpperCenter;

            EditorGUI.LabelField(minTXTRect, "min", centeredStyle);

            EditorGUI.PropertyField(minFieldRect, property.FindPropertyRelative("Min"), GUIContent.none);

            EditorGUI.LabelField(maxTXTRect, "max", centeredStyle);

            EditorGUI.PropertyField(maxFieldRect, property.FindPropertyRelative("Max"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}