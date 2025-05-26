using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using TafraKitEditor;
using TafraKit.AI3;
using UnityEngine.Analytics;
using TafraKit.GraphViews;

namespace TafraKitEditor.GraphViews
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        private Editor activeObjectEditor;
        private SerializedProperty inspectedProperty;
        private Label titleLabel;
        private VisualElement content;

        public InspectorView()
        {
            titleLabel = new Label("Select an Element");
            titleLabel.style.fontSize = 12;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
            titleLabel.pickingMode = PickingMode.Ignore;

            style.marginLeft = style.marginRight = 5f;
            Add(titleLabel);

            ScrollView scrollView = new ScrollView();

            content = scrollView;
            content.style.marginTop = 5;
            content.style.flexGrow = 1;

            Add(content);
        }

        private void ClearView()
        {
            content.Clear();

            if(activeObjectEditor != null)
            {
                UnityEngine.Object.DestroyImmediate(activeObjectEditor);
                activeObjectEditor = null;
            }

            inspectedProperty = null;

            titleLabel.text = "Select an Element";
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
            titleLabel.style.fontSize = 12;
        }

        public void InspectProperty(SerializedProperty property, string title)
        {
            ClearView();

            if(property == null)
                return;

            SerializedObject serializedObject = property.serializedObject;

            titleLabel.text = title;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.fontSize = 15;

            bool rawDisplay = false;

            FieldInfo propertyFI = property.GetFieldInfo();

            if((propertyFI != null && propertyFI.GetCustomAttribute<RawDisplayInGraphInspector>() != null) || (property.GetActualType().GetCustomAttribute<RawDisplayInGraphInspector>() != null))
                rawDisplay = true;

            if(!rawDisplay)
            {
                var propertyChildren = property.GetVisibleChildren();

                foreach(var child in propertyChildren)
                {
                    FieldInfo fi = child.GetFieldInfo();

                    if(fi != null && fi.GetCustomAttribute<HideInGraphInspector>() != null)
                        continue;

                    PropertyField pf = new PropertyField(child);
                    content.Add(pf);
                }
            }
            else
            {
                PropertyField pf = new PropertyField(property);
                content.Add(pf);
            }

            this.Bind(serializedObject);

            inspectedProperty = property;
        }
        public void InspectObject(UnityEngine.Object obj, string title)
        {
            ClearView();

            titleLabel.text = title;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.fontSize = 15;

            activeObjectEditor = Editor.CreateEditor(obj);

            IMGUIContainer container = new IMGUIContainer(() => 
            {
                activeObjectEditor.OnInspectorGUI();
            });

            Add(container);
        }

        public void UninspectProperty(SerializedProperty property)
        {
            try
            {
                if(inspectedProperty != null && inspectedProperty.propertyPath == property.propertyPath)
                    ClearView();
            }
            catch
            {
                ClearView();
            }
        }
        public void ForceUninspect()
        {
            ClearView();
        }
    }
}
