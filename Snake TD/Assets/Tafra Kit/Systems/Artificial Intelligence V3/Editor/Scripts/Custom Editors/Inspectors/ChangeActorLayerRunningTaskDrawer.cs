using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal.AI3;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    [CustomPropertyDrawer(typeof(ChangeActorLayerRunningTask))]
    public class ChangeActorLayerRunningTaskDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty layerProperty = property.FindPropertyRelative("layer");
            SerializedProperty targetThisAgentProperty = property.FindPropertyRelative("targetThisAgent");
            SerializedProperty targetActorProperty = property.FindPropertyRelative("targetActor");
            SerializedProperty changerIDProperty = property.FindPropertyRelative("changerID");

            PropertyField layerField = new PropertyField(layerProperty);
            PropertyField targetThisAgentField = new PropertyField(targetThisAgentProperty);
            PropertyField targetActorField = new PropertyField(targetActorProperty);
            PropertyField changerIDField = new PropertyField(changerIDProperty);

            targetActorField.style.display = targetThisAgentProperty.boolValue ? DisplayStyle.None : DisplayStyle.Flex;

            targetThisAgentField.RegisterValueChangeCallback((ev) => 
            {
                targetActorField.style.display = targetThisAgentProperty.boolValue ? DisplayStyle.None : DisplayStyle.Flex;
            });


            root.Add(layerField);
            root.Add(targetThisAgentField);
            root.Add(targetActorField);
            root.Add(changerIDField);

            return root;
        }
    }
}