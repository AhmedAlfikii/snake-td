using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Healthies;
using TafraKitEditor.AI3;
using TafraKitEditor.UIElement;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.Healthies
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Healthy))]
    public class HealthyEditor : Editor
    {
        private Healthy healthy;
        private SerializedProperty maxHealthProperty;
        private SerializedProperty changeLayerOnDeathProperty;
        private SerializedProperty enableSaveLoadProperty;
        private SerializedProperty saveLoadIDProperty;
        private SerializedProperty eventsProperty;
        private SerializedProperty modulesContainerProperty;


        private ClassSearchProvider modulesSearchProvider;

        private void OnEnable()
        {
            healthy = target as Healthy;

            maxHealthProperty = serializedObject.FindProperty("maxHealth");
            changeLayerOnDeathProperty = serializedObject.FindProperty("changeLayerOnDeath");
            enableSaveLoadProperty = serializedObject.FindProperty("enableSaveLoad");
            saveLoadIDProperty = serializedObject.FindProperty("saveLoadID");
            eventsProperty = serializedObject.FindProperty("events");
            modulesContainerProperty = serializedObject.FindProperty("modulesContainer");

            modulesSearchProvider = ClassSearchProvider.CreateOrGetInstance();
            modulesSearchProvider.Initialize(typeof(HealthyModule), "Healthy Modules", OnSearchMenuModuleSelected);
        }

        private void OnSearchMenuModuleSelected(Type type, SearchWindowContext context)
        {

        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            if (Application.isPlaying && targets.Length == 1)
                CreateHealthBar(root);

            PropertyField damagePointPF = new PropertyField(serializedObject.FindProperty("damagePoint"));
            root.Add(damagePointPF);

            Label healthLabel = CreateHeaderLabel("Health");
            root.Add(healthLabel);

            PropertyField maxHealthPF = new PropertyField(maxHealthProperty);
            root.Add(maxHealthPF);
            PropertyField maxHealthoPF = new PropertyField(serializedObject.FindProperty("maxHealtho"));
            root.Add(maxHealthoPF);

            PropertyField changeLayerOnDeathPF = new PropertyField(changeLayerOnDeathProperty);
            PropertyField deathLayerPF = new PropertyField(serializedObject.FindProperty("deathLayer"));
            deathLayerPF.style.display = changeLayerOnDeathProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            changeLayerOnDeathPF.RegisterCallback<ChangeEvent<bool>>((ev) =>
            {
                deathLayerPF.style.display = ev.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
            PropertyField listenToMaxHealthChangesPF = new PropertyField(serializedObject.FindProperty("listenToMaxHealthChanges"));
            root.Add(changeLayerOnDeathPF);
            root.Add(deathLayerPF);
            root.Add(listenToMaxHealthChangesPF);

            Label saveLoadLabel = CreateHeaderLabel("Save & Load");
            root.Add(saveLoadLabel);

            VisualElement saveLoadContent = new VisualElement();
            PropertyField enableSaveLoadPF = new PropertyField(enableSaveLoadProperty);
            enableSaveLoadPF.TieVisualElementsToBoolField(enableSaveLoadProperty.boolValue, true, saveLoadContent);
          
            root.Add(enableSaveLoadPF);
            root.Add(saveLoadContent);

            HelpBox noIDHelpBox = new HelpBox("You need to set an ID to correctly save and load.", HelpBoxMessageType.Error);
            noIDHelpBox.style.display = string.IsNullOrEmpty(saveLoadIDProperty.stringValue) ? DisplayStyle.Flex : DisplayStyle.None;

            PropertyField saveLoadIDPF = new PropertyField(saveLoadIDProperty);
            saveLoadIDPF.RegisterCallback<ChangeEvent<string>>((ev) =>
            {
                noIDHelpBox.style.display = string.IsNullOrEmpty(ev.newValue) ? DisplayStyle.Flex : DisplayStyle.None;
            });

            saveLoadContent.Add(noIDHelpBox);
            saveLoadContent.Add(saveLoadIDPF);

            PropertyField eventsPF = new PropertyField(eventsProperty);
            root.Add(eventsPF);

            PropertyField modulesContainerPF = new PropertyField(modulesContainerProperty);
            root.Add(modulesContainerPF);

            return root;
        }

        private Label CreateHeaderLabel(string text)
        {
            Label label = new Label(text);
            label.style.fontSize = 12;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginTop = 5;
            label.style.marginBottom = 2;

            return label;
        }
        private void CreateHealthBar(VisualElement root)
        {
            VisualElement healthbarContainer = new VisualElement();
            healthbarContainer.style.height = 70;
            healthbarContainer.style.justifyContent = Justify.Center;
            healthbarContainer.style.alignItems = Align.Center;
            root.Add(healthbarContainer);

            VisualElement healthbarFrame = new VisualElement();
            healthbarFrame.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1);
            healthbarFrame.style.height = 30;
            healthbarFrame.style.width = new StyleLength(new Length(50f, LengthUnit.Percent));
            healthbarFrame.style.SetBorderRadius(15);
            healthbarFrame.style.justifyContent = Justify.Center;
            healthbarContainer.Add(healthbarFrame);

            VisualElement healthbarBG = new VisualElement();
            healthbarBG.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
            healthbarBG.style.flexGrow = 1;
            healthbarBG.style.SetMargins(3);
            healthbarBG.style.SetBorderRadius(12);
            healthbarFrame.Add(healthbarBG);

            VisualElement healthbar = new VisualElement();
            healthbar.style.backgroundColor = new Color(227 / 255f, 91 / 255f, 91 / 255f, 1);
            healthbar.style.flexGrow = 1;
            healthbar.style.SetBorderRadius(12);
            healthbarBG.Add(healthbar);
            if(healthy.IsInitialized)
                healthbar.style.width = new StyleLength(new Length((healthy.CurrentHealth / healthy.CurrentMaxHealth) * 100, LengthUnit.Percent));

            float curHealth = 0;
            float curMaxHealth = 0;
            if(healthy.IsInitialized)
            {
                curHealth = healthy.CurrentHealth;
                curMaxHealth = healthy.CurrentMaxHealth;
            }

            Label healthNumberLabel = new Label($"{curHealth}/{curMaxHealth}");
            healthNumberLabel.style.position = Position.Absolute;
            healthNumberLabel.style.alignSelf = Align.Center;
            healthNumberLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            healthNumberLabel.style.fontSize = 15;
            healthNumberLabel.style.color = new Color(0.9f, 0.9f, 0.9f, 1);
            healthbarFrame.Add(healthNumberLabel);

            healthbarContainer.schedule.Execute(() =>
            {
                if(!healthy.IsInitialized)
                    return;

                healthNumberLabel.text = $"{healthy.CurrentHealth}/{healthy.CurrentMaxHealth}";
                healthbar.style.width = new StyleLength(new Length((healthy.CurrentHealth / healthy.CurrentMaxHealth) * 100, LengthUnit.Percent));
            }).Every(100);
        }
    }
}