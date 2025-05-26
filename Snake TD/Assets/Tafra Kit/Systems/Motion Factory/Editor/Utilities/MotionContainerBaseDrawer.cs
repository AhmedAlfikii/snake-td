using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Threading.Tasks;

namespace TafraKitEditor.MotionFactory
{
    public abstract class MotionContainerBaseDrawer : PropertyDrawer
    {
        public VisualTreeAsset uxml;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializeReferenceListContainerField containerField = new SerializeReferenceListContainerField(property, "motions", true, "Motion", "Motions", property.tooltip, true);

            return containerField;
        }
    }
}