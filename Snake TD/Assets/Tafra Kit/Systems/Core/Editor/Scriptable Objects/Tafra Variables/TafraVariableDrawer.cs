using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(TafraVariable<,>), true)]
    public class TafraVariableDrawer : PropertyDrawer
    { 
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new TafraVariableAssetField(property, property.displayName, property.tooltip);
        }
    }
}