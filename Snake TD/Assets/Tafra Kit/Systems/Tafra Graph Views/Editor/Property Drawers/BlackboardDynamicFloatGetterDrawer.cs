using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.GraphViews
{
    [CustomPropertyDrawer(typeof(BlackboardDynamicFloatGetter))]
    public class BlackboardDynamicFloatGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {

            PropertyBox propertyBox = new PropertyBox(property, property.displayName, "", property.tooltip, true);

            SerializedProperty floatTypeProperty = property.FindPropertyRelative("floatType");
            SerializedProperty normalFloatGetterProperty = property.FindPropertyRelative("normalFloatGetter");
            SerializedProperty advancedFloatGetterProperty = property.FindPropertyRelative("advancedFloatGetter");

            PropertyField normalFloatGetterPF = new PropertyField(normalFloatGetterProperty);
            PropertyField advancedFloatGetterPF = new PropertyField(advancedFloatGetterProperty);

            BlackboardDynamicFloatGetter.FloatType selectedType = ((BlackboardDynamicFloatGetter.FloatType)floatTypeProperty.enumValueIndex);

            normalFloatGetterPF.style.display = selectedType == BlackboardDynamicFloatGetter.FloatType.NormalFloat ? DisplayStyle.Flex : DisplayStyle.None;
            advancedFloatGetterPF.style.display = selectedType == BlackboardDynamicFloatGetter.FloatType.TafraAdavncedFloat ? DisplayStyle.Flex : DisplayStyle.None;

            PropertyField floatTypePF = new PropertyField(floatTypeProperty, "");
            floatTypePF.RegisterValueChangeCallback((ev) =>
            {
                BlackboardDynamicFloatGetter.FloatType selectedType = ((BlackboardDynamicFloatGetter.FloatType)floatTypeProperty.enumValueIndex);

                normalFloatGetterPF.style.display = selectedType == BlackboardDynamicFloatGetter.FloatType.NormalFloat ? DisplayStyle.Flex : DisplayStyle.None;
                advancedFloatGetterPF.style.display = selectedType == BlackboardDynamicFloatGetter.FloatType.TafraAdavncedFloat ? DisplayStyle.Flex : DisplayStyle.None;
            });

            propertyBox.Header.Add(floatTypePF);
            propertyBox.Content.Add(normalFloatGetterPF);
            propertyBox.Content.Add(advancedFloatGetterPF);

            return propertyBox;
        }
    }
}