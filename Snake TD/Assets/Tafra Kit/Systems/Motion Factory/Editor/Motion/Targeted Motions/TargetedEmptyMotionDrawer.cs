using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TafraKit.MotionFactory;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.MotionFactory
{

    [CustomPropertyDrawer(typeof(TargetedEmptyMotion))]
    public class TargetedEmptyMotionDrawer : TargetedMotionDrawer
    {
        protected override string Name => "Empty Motion";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/UI Toolkit Assets/UXML/Motion Factory/BaseMotionEditor.uxml");
            uxml.CloneTree(root);
            VisualElement motionPropertiesBox = root.Q<VisualElement>("MotionPropertiesBox");
            VisualElement targetsBox = root.Q<VisualElement>("TargetsBox");
            VisualElement unAssignedReferenesBox = root.Q<VisualElement>("UnassignedReferencesBox");
            Foldout references = root.Q<Foldout>("References");

            targetsBox.style.display = DisplayStyle.None;
            unAssignedReferenesBox.style.display = DisplayStyle.None;
            references.style.display = DisplayStyle.None;

            Label titleLabel = root.Q<Label>("Title");
            titleLabel.text = Name;
            Label propertyNameLabel = root.Q<Label>("PropertyName");
            propertyNameLabel.text = property.displayName;

            IEnumerable<SerializedProperty> propertyChildren = property.GetVisibleChildren();
            for(int i = 0; i < propertyChildren.Count(); i++)
            {
                SerializedProperty p = propertyChildren.ElementAt(i);
                PropertyField pf = new PropertyField(p);

                if (p.name != "delay" && p.name != "easing" && p.name != "targetInitialState")
                    motionPropertiesBox.Add(pf);
            }

            OnCreated(property);

            return root;
        }
    }
}