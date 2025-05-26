using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.GraphViews;
using UnityEngine.UIElements;
using TafraKit;

namespace TafraKitEditor.GraphViews
{
    [CustomPropertyDrawer(typeof(BlackboardFloatSetter))]
    public class BlackboardFloatSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<float>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardAdvancedFloatSetter))]
    public class BlackboardAdvancedFloatSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<TafraAdvancedFloat>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardIntSetter))]
    public class BlackboardIntSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<int>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardBoolSetter))]
    public class BlackboardBoolSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<bool>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardStringSetter))]
    public class BlackboardStringSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<string>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardVector3Setter))]
    public class BlackboardVector3SetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<Vector3>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardGameObjectSetter))]
    public class BlackboardGameObjectSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<GameObject>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardScriptableObjectSetter))]
    public class BlackboardScriptableObjectSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<ScriptableObject>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardActorSetter))]
    public class BlackboardActorSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<TafraActor>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardObjectSetter))]
    public class BlackboardObjectSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<UnityEngine.Object>(property, false);
        }
    }
    [CustomPropertyDrawer(typeof(BlackboardSystemObjectSetter))]
    public class BlackboardSystemObjectSetterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new BlackboardPropertyReferenceField<object>(property, false);
        }
    }
}