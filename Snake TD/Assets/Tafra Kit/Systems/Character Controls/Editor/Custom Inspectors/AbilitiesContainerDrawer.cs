using System.Collections;
using System.Collections.Generic;
using TafraKit;
using TafraKit.CharacterControls;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.CharacterControls
{
    [CustomPropertyDrawer(typeof(AbilitiesContainer))]
    public class AbilitiesContainerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            CollapsibleContainer root = new CollapsibleContainer(property.displayName, property, false);

            SerializedProperty enableSaveAndLoadProperty = property.FindPropertyRelative("enableSaveAndLoad");
            SerializedProperty idProperty = property.FindPropertyRelative("id");
            SerializedProperty resourcesPathProperty = property.FindPropertyRelative("resourcesPath");
            SerializedProperty slotsProperty = property.FindPropertyRelative("slots");
            SerializedProperty defaultAbilitiesProperty = property.FindPropertyRelative("defaultAbilities");

            SerializedProperty equippedAbilitiesProperty = property.FindPropertyRelative("equippedAbilities");

            PropertyField enableSaveAndLoadPropertyField = new PropertyField(enableSaveAndLoadProperty);
            PropertyField idPropertyField = new PropertyField(idProperty);
            PropertyField resourcesPathPropertyField = new PropertyField(resourcesPathProperty);
            PropertyField slotsPropertyField = new PropertyField(slotsProperty);
            PropertyField defaultAbilitiesPropertyField = new PropertyField(defaultAbilitiesProperty);

            PropertyField equippedAbilitiesPropertyField = new PropertyField(equippedAbilitiesProperty);


            enableSaveAndLoadPropertyField.RegisterValueChangeCallback((evt) =>
            {
                idPropertyField.style.display = enableSaveAndLoadProperty.boolValue == true ? DisplayStyle.Flex : DisplayStyle.None;
            });

            idPropertyField.style.display = enableSaveAndLoadProperty.boolValue == true ? DisplayStyle.Flex : DisplayStyle.None;

            root.Content.Add(enableSaveAndLoadPropertyField);
            root.Content.Add(idPropertyField);
            root.Content.Add(resourcesPathPropertyField);
            root.Content.Add(slotsPropertyField);
            root.Content.Add(defaultAbilitiesPropertyField);
            root.Content.Add(equippedAbilitiesPropertyField);

            if(Application.isPlaying)
            {
                object propertyValue = property.GetActualValue();

                if(propertyValue != null && propertyValue is AbilitiesContainer ac)
                {
                    CollapsibleContainer equippedAbilitiesBox = new CollapsibleContainer("Equipped Abilities", property.propertyPath + "_EquippedAbilities", true);
                    
                    root.Content.Add(equippedAbilitiesBox);

                    for(int i = 0; i < ac.EquippedAbilities.Count; i++)
                    {
                        Ability ability = ac.EquippedAbilities[i];

                        if(ability == null)
                        {
                            TafraDebugger.Log("Abilities Container", "The ability you're trying to acces has been destroyed.", TafraDebugger.LogType.Warning);
                            continue;
                        }

                        Button abButton = new Button(() => {
                            AssetDatabase.OpenAsset(ability.GetInstanceID());
                        });
                        abButton.text = ability.name;
                        equippedAbilitiesBox.Content.Add(abButton);
                    }
                }
            }

            return root;
        }

    }
}