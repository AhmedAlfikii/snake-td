using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.GraphViews
{
    [CustomPropertyDrawer(typeof(BlackboardDynamicPointGetter))]
    public class BlackboardDynamicPointGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {

            PropertyBox propertyBox = new PropertyBox(property, property.displayName, "", property.tooltip, true);

            SerializedProperty pointTypeProperty = property.FindPropertyRelative("pointType");
            SerializedProperty actorGetterProperty = property.FindPropertyRelative("actorGetter");
            SerializedProperty gameObjectGetterProperty = property.FindPropertyRelative("gameObjectGetter");
            SerializedProperty vector3GetterProperty = property.FindPropertyRelative("vector3Getter");
            SerializedProperty getHealthyTargetPointProperty = property.FindPropertyRelative("getHealthyTargetPoint");

            PropertyField actorGetterPF = new PropertyField(actorGetterProperty);
            PropertyField gameObjectGetterPF = new PropertyField(gameObjectGetterProperty);
            PropertyField vector3GetterPF = new PropertyField(vector3GetterProperty);
            PropertyField getHealthyTargetPointPF = new PropertyField(getHealthyTargetPointProperty);

            BlackboardDynamicPointGetter.PointType selectedType = ((BlackboardDynamicPointGetter.PointType)pointTypeProperty.enumValueIndex);

            actorGetterPF.style.display = selectedType == BlackboardDynamicPointGetter.PointType.Actor ? DisplayStyle.Flex : DisplayStyle.None;
            gameObjectGetterPF.style.display = selectedType == BlackboardDynamicPointGetter.PointType.GameObject ? DisplayStyle.Flex : DisplayStyle.None;
            vector3GetterPF.style.display = selectedType == BlackboardDynamicPointGetter.PointType.Vector3 ? DisplayStyle.Flex : DisplayStyle.None;
            getHealthyTargetPointPF.style.display = (selectedType == BlackboardDynamicPointGetter.PointType.Actor || selectedType == BlackboardDynamicPointGetter.PointType.ThisAgent) ? DisplayStyle.Flex : DisplayStyle.None;

            PropertyField floatTypePF = new PropertyField(pointTypeProperty, "");
            floatTypePF.RegisterValueChangeCallback((ev) =>
            {
                BlackboardDynamicPointGetter.PointType selectedType = ((BlackboardDynamicPointGetter.PointType)pointTypeProperty.enumValueIndex);

                actorGetterPF.style.display = selectedType == BlackboardDynamicPointGetter.PointType.Actor ? DisplayStyle.Flex : DisplayStyle.None;
                gameObjectGetterPF.style.display = selectedType == BlackboardDynamicPointGetter.PointType.GameObject ? DisplayStyle.Flex : DisplayStyle.None;
                vector3GetterPF.style.display = selectedType == BlackboardDynamicPointGetter.PointType.Vector3 ? DisplayStyle.Flex : DisplayStyle.None;
                getHealthyTargetPointPF.style.display = (selectedType == BlackboardDynamicPointGetter.PointType.Actor || selectedType == BlackboardDynamicPointGetter.PointType.ThisAgent) ? DisplayStyle.Flex : DisplayStyle.None;
            });

            propertyBox.Header.Add(floatTypePF);
            propertyBox.Content.Add(getHealthyTargetPointPF);
            propertyBox.Content.Add(actorGetterPF);
            propertyBox.Content.Add(gameObjectGetterPF);
            propertyBox.Content.Add(vector3GetterPF);

            return propertyBox;
        }
    }
}