using TafraKit.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.UI
{
    [CustomEditor(typeof(ScrollDynamicMask))]
    public class ScrollDynamicMaskEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            PropertyField softnessField = new PropertyField(serializedObject.FindProperty("m_Softness"));
            PropertyField paddingField = new PropertyField(serializedObject.FindProperty("m_Padding"), "Preview Padding");
            PropertyField fadedPaddingField = new PropertyField(serializedObject.FindProperty("fadedPadding"));
            PropertyField normalPaddingField = new PropertyField(serializedObject.FindProperty("normalPadding"));
            PropertyField fadeDurationField = new PropertyField(serializedObject.FindProperty("fadeDuration"));

            root.Add(softnessField);
            root.Add(paddingField);
            root.Add(fadedPaddingField);
            root.Add(normalPaddingField);
            root.Add(fadeDurationField);

            return root;
        }
    }
}