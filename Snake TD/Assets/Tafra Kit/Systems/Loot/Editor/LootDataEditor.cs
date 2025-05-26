using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.Loot;
using UnityEngine.UIElements;

namespace TafraKitEditor.Loot
{
    //[CustomEditor(typeof(TafraKit.Loot.LootData))]
    public class LootDataEditor : Editor
    {
        protected IEnumerable<SerializedProperty> allSerializedProperties;
        protected SerializedProperty rewardTypeProperty;

        private void OnEnable()
        {
            allSerializedProperties = serializedObject.GetVisibleProperties();

            rewardTypeProperty = serializedObject.FindProperty("rewardType");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            for (int i = 0; i < allSerializedProperties.Count(); i++)
            {
                SerializedProperty property = allSerializedProperties.ElementAt(i);

                if (property.name == "scriptableFloatChange" && rewardTypeProperty.enumValueIndex > 9)
                    continue;
                 
                EditorGUILayout.PropertyField(property);
            }

            serializedObject.ApplyModifiedProperties();
        }
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
    }
}