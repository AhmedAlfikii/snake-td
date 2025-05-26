using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.Consumables;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.ResourcesSystem
{
    [CustomEditor(typeof(Consumable))]
    public class ConsumableEditor : ScriptableFloatEditor
    {
        private SerializedProperty displayNamesProperty;
        private SerializedProperty iconsProperty;
        private SerializedProperty colorsProperty;
        private SerializedProperty descriptionProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            displayNamesProperty = serializedObject.FindProperty("displayNames");
            iconsProperty = serializedObject.FindProperty("icons");
            colorsProperty = serializedObject.FindProperty("colors");
            descriptionProperty = serializedObject.FindProperty("description");
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.name = "root";

            Label header = new Label("Consumable");
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginTop = 5;

            PropertyField displayNamesPF = new PropertyField(serializedObject.FindProperty("displayNames"));
            PropertyField iconsPF = new PropertyField(serializedObject.FindProperty("icons"));
            PropertyField colorsPF = new PropertyField(serializedObject.FindProperty("colors"));
            PropertyField descriptionPF = new PropertyField(serializedObject.FindProperty("description"));

            IMGUIContainer sfContainer = new IMGUIContainer(() => {
                serializedObject.Update();
                BuildSFUI();
                serializedObject.ApplyModifiedProperties();
            });

            root.Add(header);

            root.Add(displayNamesPF);
            root.Add(iconsPF);
            root.Add(colorsPF);
            root.Add(descriptionPF);

            root.Add(sfContainer);

            return root;
        }
    }
}