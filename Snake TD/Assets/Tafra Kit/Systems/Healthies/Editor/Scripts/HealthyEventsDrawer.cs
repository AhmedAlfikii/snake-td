using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal.Healthies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.Healthies
{
    [CustomPropertyDrawer(typeof(HealthyEvents))]
    public class HealthyEventsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            root.style.marginTop = root.style.marginBottom = 5;

            Foldout foldout = new Foldout();
            foldout.value = false;
            foldout.viewDataKey = property.propertyPath;
            foldout.text = property.displayName;
            foldout.Q<Label>().style.unityFontStyleAndWeight = FontStyle.Bold;

            root.Add(foldout);

            DrawControlEventPair("OnInitialize", "EnableOnInitializeEvent", foldout, property);
            DrawControlEventPair("OnAboutToDie", "EnableOnAboutToDieEvent", foldout, property);
            DrawControlEventPair("OnDeath", "EnableOnDeathEvent", foldout, property);
            DrawControlEventPair("OnTakenDamage", "EnableOnTakenDamageEvent", foldout, property);
            DrawControlEventPair("OnAboutToHeal", "EnableOnAboutToHealEvent", foldout, property);
            DrawControlEventPair("OnHeal", "EnableOnHealEvent", foldout, property);
            DrawControlEventPair("OnHealthChange", "EnableOnHealthChangeEvent", foldout, property);
            DrawControlEventPair("OnMaxHealthChange", "EnableOnMaxHealthChangeEvent", foldout, property);
            DrawControlEventPair("OnRevive", "EnableOnReviveEvent", foldout, property);

            return root;
        }

        private void DrawControlEventPair(string eventPropretyName, string controlPropertyName, VisualElement container, SerializedProperty property)
        {
            SerializedProperty eventProperty = property.FindPropertyRelative(eventPropretyName);
            SerializedProperty controlProperty = property.FindPropertyRelative(controlPropertyName);

            PropertyField eventPF = new PropertyField(eventProperty);
            eventPF.style.display = controlProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
         
            PropertyField controlPF = new PropertyField(controlProperty);
            controlPF.RegisterCallback<ChangeEvent<bool>>((ev) =>
            {
                eventPF.style.display = ev.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
        
            container.Add(controlPF);
            container.Add(eventPF);
        }
    }
}