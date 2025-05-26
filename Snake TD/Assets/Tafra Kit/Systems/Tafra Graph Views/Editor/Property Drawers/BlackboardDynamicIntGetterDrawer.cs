using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.GraphViews
{
    [CustomPropertyDrawer(typeof(BlackboardDynamicIntGetter))]
    public class BlackboardDynamicIntGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {

            PropertyBox propertyBox = new PropertyBox(property, property.displayName, "", property.tooltip, true);

            SerializedProperty intTypeProperty = property.FindPropertyRelative("intType");
            SerializedProperty normalIntGetterProperty = property.FindPropertyRelative("normalIntGetter");
            SerializedProperty advancedFloatGetterProperty = property.FindPropertyRelative("advancedFloatGetter");

            PropertyField normalIntGetterPF = new PropertyField(normalIntGetterProperty);
            PropertyField advancedFloatGetterPF = new PropertyField(advancedFloatGetterProperty);

            BlackboardDynamicIntGetter.IntType selectedType = ((BlackboardDynamicIntGetter.IntType)intTypeProperty.enumValueIndex);

            normalIntGetterPF.style.display = selectedType == BlackboardDynamicIntGetter.IntType.NormalInt ? DisplayStyle.Flex : DisplayStyle.None;
            advancedFloatGetterPF.style.display = selectedType == BlackboardDynamicIntGetter.IntType.TafraAdavncedFloat ? DisplayStyle.Flex : DisplayStyle.None;

            PropertyField floatTypePF = new PropertyField(intTypeProperty, "");
            floatTypePF.RegisterValueChangeCallback((ev) =>
            {
                BlackboardDynamicIntGetter.IntType selectedType = ((BlackboardDynamicIntGetter.IntType)intTypeProperty.enumValueIndex);

                normalIntGetterPF.style.display = selectedType == BlackboardDynamicIntGetter.IntType.NormalInt ? DisplayStyle.Flex : DisplayStyle.None;
                advancedFloatGetterPF.style.display = selectedType == BlackboardDynamicIntGetter.IntType.TafraAdavncedFloat ? DisplayStyle.Flex : DisplayStyle.None;
            });

            propertyBox.Header.Add(floatTypePF);
            propertyBox.Content.Add(normalIntGetterPF);
            propertyBox.Content.Add(advancedFloatGetterPF);

            return propertyBox;
        }
    }
}