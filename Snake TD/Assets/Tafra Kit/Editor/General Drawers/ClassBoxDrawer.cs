using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using TafraKit;

namespace TafraKitEditor
{
    public abstract class ClassBoxDrawer : PropertyDrawer
    {
        protected abstract string UXMLPathOverride { get; }
        protected abstract string Title { get; }
        protected abstract Color MainColor { get; }

      
        private string defaultUXMLPath = "Assets/Tafra Kit/UI Toolkit Assets/UXML/General/ClassBox.uxml";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(string.IsNullOrEmpty(UXMLPathOverride) ? defaultUXMLPath : UXMLPathOverride);
            uxml.CloneTree(root);

            Label titleLabel = root.Q<Label>("Title");
            titleLabel.text = Title;
            titleLabel.style.color = MainColor;

            VisualElement propertiesBox = root.Q<VisualElement>("PropertiesBox");

            IEnumerable<SerializedProperty> properties = property.GetVisibleChildren();

            foreach(var p in properties)
            {
                PropertyField pf = new PropertyField();
                pf.bindingPath = p.propertyPath;
                pf.name = p.name;
                propertiesBox.Add(pf);
            }

            return root;
        }
    }
}