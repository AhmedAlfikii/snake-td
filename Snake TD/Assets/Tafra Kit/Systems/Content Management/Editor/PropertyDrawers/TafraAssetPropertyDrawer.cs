using TafraKit.ContentManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.ContentManagement
{
    [CustomPropertyDrawer(typeof(TafraAsset<>))]
    public class TafraAssetPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new TafraAssetField(property, property.displayName, property.tooltip);
        }
    }
}