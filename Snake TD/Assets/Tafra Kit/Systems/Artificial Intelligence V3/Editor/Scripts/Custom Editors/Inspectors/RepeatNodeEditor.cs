using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal.AI3;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    [CustomPropertyDrawer(typeof(RepeatNode))]
    public class RepeatNodeEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty repeatRuleProperty = property.FindPropertyRelative("repeatRule");
            SerializedProperty executionsCountProperty = property.FindPropertyRelative("executionsCount");
            SerializedProperty failIfChildFailedProperty = property.FindPropertyRelative("failIfChildFailed");
            SerializedProperty repeatConditionsProperty = property.FindPropertyRelative("repeatConditions");

            PropertyField executionsCountField = new PropertyField(executionsCountProperty);
            executionsCountField.style.display = ((RepeatNode.RepeatRules)repeatRuleProperty.enumValueIndex) == RepeatNode.RepeatRules.Count ? DisplayStyle.Flex : DisplayStyle.None;
            PropertyField failIfChildFailedField = new PropertyField(failIfChildFailedProperty);
            failIfChildFailedField.style.display = ((RepeatNode.RepeatRules)repeatRuleProperty.enumValueIndex) == RepeatNode.RepeatRules.Count ? DisplayStyle.Flex : DisplayStyle.None;

            PropertyField repeatRuleField = new PropertyField(repeatRuleProperty);
            repeatRuleField.RegisterValueChangeCallback((ev) => 
            {
                executionsCountField.style.display = ((RepeatNode.RepeatRules)repeatRuleProperty.enumValueIndex) == RepeatNode.RepeatRules.Count ? DisplayStyle.Flex : DisplayStyle.None;
                failIfChildFailedField.style.display = ((RepeatNode.RepeatRules)repeatRuleProperty.enumValueIndex) == RepeatNode.RepeatRules.Count ? DisplayStyle.Flex : DisplayStyle.None;
            });

            PropertyField repeatConditionsField = new PropertyField(repeatConditionsProperty);

            root.Add(repeatRuleField);
            root.Add(executionsCountField);
            root.Add(failIfChildFailedField);
            root.Add(repeatConditionsField);

            return root;
        }
    }
}