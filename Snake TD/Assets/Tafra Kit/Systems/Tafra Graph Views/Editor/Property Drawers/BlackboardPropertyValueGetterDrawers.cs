using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.GraphViews;
using UnityEngine.UIElements;
using TafraKit;
using ZUI;
using UnityEditor.UIElements;

namespace TafraKitEditor.GraphViews
{
    [CustomPropertyDrawer(typeof(BlackboardFloatGetter))]
    public class BlackboardFloatGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<float>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardAdvancedFloatGetter))]
    public class BlackboardAdvancedFloatGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<TafraAdvancedFloat>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardIntGetter))]
    public class BlackboardIntGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<int>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardBoolGetter))]
    public class BlackboardBoolGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<bool>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardStringGetter))]
    public class BlackboardStringGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<string>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardVector3Getter))]
    public class BlackboardVector3GetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<Vector3>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardGameObjectGetter))]
    public class BlackboardGameObjectGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<GameObject>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardScriptableObjectGetter))]
    public class BlackboardScriptableObjectGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<ScriptableObject>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardActorGetter))]
    public class BlackboardActorGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            BlackboardPropertyReferenceField<TafraActor> field = new BlackboardPropertyReferenceField<TafraActor>(property, true);

            SerializedProperty targetIsThisActorProperty = property.FindPropertyRelative("targetIsThisActor");

            if(targetIsThisActorProperty.boolValue)
            {
                field.PropertyBox.MainContainer.Q<VisualElement>("content").style.display = DisplayStyle.None;
                field.Q<VisualElement>("target-blackboard").style.display = DisplayStyle.None;
            }
            else
            {
                field.PropertyBox.MainContainer.Q<VisualElement>("content").style.display = DisplayStyle.Flex;
                field.Q<VisualElement>("target-blackboard").style.display = DisplayStyle.Flex;
            }

            PropertyField targetIsThisActorField = new PropertyField(targetIsThisActorProperty);
            targetIsThisActorField.RegisterValueChangeCallback((ev) =>
            {
                if(ev.changedProperty.boolValue)
                {
                    field.PropertyBox.MainContainer.Q<VisualElement>("content").style.display = DisplayStyle.None;
                    field.Q<VisualElement>("target-blackboard").style.display = DisplayStyle.None;
                }
                else
                {
                    field.PropertyBox.MainContainer.Q<VisualElement>("content").style.display = DisplayStyle.Flex;
                    field.Q<VisualElement>("target-blackboard").style.display = DisplayStyle.Flex;
                }
            });

            field.PropertyBox.MainContainer.Add(targetIsThisActorField);

            return field;
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardObjectGetter))]
    public class BlackboardObjectGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<UnityEngine.Object>(property, true);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardSystemObjectGetter))]
    public class BlackboardSystemObjectGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            BlackboardPropertyReferenceField<object> field = new BlackboardPropertyReferenceField<object>(property, true);

            HelpBox helpBox = new HelpBox("Raw system objects can't have default value. Define a type if you need to set one.", HelpBoxMessageType.Info);

            field.PropertyBox.MainContainer.Insert(2, helpBox);
            root.Add(field);

            return root;
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardSystemObjectGetter<>))]
    public class BlackboardGeneralObjectGetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<object>(property, true);
        }
    }
}