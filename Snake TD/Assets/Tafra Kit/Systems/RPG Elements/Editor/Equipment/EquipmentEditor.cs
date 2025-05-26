using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TafraKit.RPG;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.RPGElements
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(Equipment), true)]
    public class EquipmentEditor : Editor
    {
        private IEnumerable<SerializedProperty> properties;

        private void OnEnable()
        {
            properties = serializedObject.GetVisibleProperties();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool multiTargets = targets.Length > 1;
            EditorGUILayout.LabelField("ID", $"{(multiTargets ? "-" : target.name)}");

            if(properties != null)
            {
                for (int i = 0; i < properties.Count(); i++)
                {
                    SerializedProperty property = properties.ElementAt(i);

                    if(property.name != "id" && property.name != "m_Script")
                        EditorGUILayout.PropertyField(property);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            bool multiTargets = targets.Length > 1;

            TextField textField = new TextField("ID");
            textField.SetEnabled(false);
            textField.SetValueWithoutNotify($"{(multiTargets ? "-" : target.name)}");
            root.Add(textField);

            if(properties != null)
            {
                for(int i = 0; i < properties.Count(); i++)
                {
                    SerializedProperty property = properties.ElementAt(i);

                    if(property.name != "id" && property.name != "m_Script")
                    {
                        PropertyField pf = new PropertyField(property);
                        root.Add(pf);
                    }
                }
            }

            return root;
        }
    }
}