using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.UI;
using UnityEngine.UIElements;

namespace TafraKitEditor.UI
{
    [CustomPropertyDrawer(typeof(UISwappableValuesContainer))]
    public class UISwappableValuesContainerDrawer : ReorderableListContainerDrawer
    {
        protected override Type SupportedType => typeof(UISwappableValues);
        protected override string ListPropertyName => "swappableValues";
        protected override string UXMLPathOverride => null;
        protected override string SingleElementName => "Swappable Values";
        protected override string PluralElementName => "Swappable Values";
        protected override bool ExpandedByDefault => false;
        protected override Color AddButtonColor => new Color(0.3372549f, 0.5647059f, 0.3215686f, 1);
        protected override Color ElementBackgroundColor => new Color(0.3490621f, 0.425f, 0.343825f, 1f);
        protected override Color ElementBorderColor => new Color(0.1f, 0.1f, 0.1f, 1f);
        protected override bool IsListSerialziedByReference => true;
        protected override bool MaintainUniqueTypeElements => false;
        protected override bool OverrideElementDrawer => true;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return base.CreatePropertyGUI(property);
        }
    }
}