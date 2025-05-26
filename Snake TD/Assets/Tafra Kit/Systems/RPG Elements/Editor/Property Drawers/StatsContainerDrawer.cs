using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using TafraKit.RPG;
using UnityEditor.UIElements;
using TafraKit.Internal.RPGElements;
using System.Reflection;
using System;

namespace TafraKitEditor.RPGElements
{
    [CustomPropertyDrawer(typeof(StatsContainer))]
    public class StatsContainerDrawer : PropertyDrawer
    {
        private Foldout previewFoldout;
        private Dictionary<Stat, VisualElement> statFieldByStat = new Dictionary<Stat, VisualElement>();

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            CollapsibleContainer container = new CollapsibleContainer(property.displayName, property, false, false);

            VisualElement contentContainer = new VisualElement();
            contentContainer.style.marginLeft = contentContainer.style.marginRight = 10f;

            container.Content.Add(contentContainer);

            SerializedProperty baseStatObjectsProperty = property.FindPropertyRelative("baseStatObjects");
            SerializedProperty statCollectionAccessoriesProperty = property.FindPropertyRelative("statCollectionAccessories");
            SerializedProperty onStatAddedProperty = property.FindPropertyRelative("onStatAdded");
            SerializedProperty onStatRemovedProperty = property.FindPropertyRelative("onStatRemoved");
            SerializedProperty onStatsUpdatedProperty = property.FindPropertyRelative("onStatsUpdated");
            SerializedProperty statCollectionsProperty = property.FindPropertyRelative("statCollections");

            PropertyField baseStatObjectsField = new PropertyField(baseStatObjectsProperty);
            PropertyField statCollectionAccessoriesField = new PropertyField(statCollectionAccessoriesProperty);
            PropertyField onStatAddedField = new PropertyField(onStatAddedProperty);
            PropertyField onStatRemovedField = new PropertyField(onStatRemovedProperty);
            PropertyField onStatsUpdatedField = new PropertyField(onStatsUpdatedProperty);
            PropertyField statCollectionsField = new PropertyField(statCollectionsProperty);

            contentContainer.Add(baseStatObjectsField);
            contentContainer.Add(statCollectionAccessoriesField);
            contentContainer.Add(statCollectionsField);

            Foldout eventsFoldout = new Foldout();
            eventsFoldout.style.marginTop = 15;
            eventsFoldout.value = false;
            eventsFoldout.viewDataKey = property.propertyPath + "_events";
            eventsFoldout.text = "Events";
            eventsFoldout.Q<Label>().style.unityFontStyleAndWeight = FontStyle.Bold;

            eventsFoldout.Add(onStatAddedField);
            eventsFoldout.Add(onStatRemovedField);
            eventsFoldout.Add(onStatsUpdatedField);

            contentContainer.Add(eventsFoldout);

            #region Preview
            if(Application.isPlaying)
            {
                previewFoldout = new Foldout();
                previewFoldout.style.marginTop = 15;
                previewFoldout.style.color = new Color(0.57f, 0.9f, 1, 1);
                previewFoldout.value = false;
                previewFoldout.viewDataKey = property.propertyPath + "_preview";
                previewFoldout.text = "Preview";
                previewFoldout.Q<Label>().style.unityFontStyleAndWeight = FontStyle.Bold;
                contentContainer.Add(previewFoldout);

                statFieldByStat.Clear();

                FieldInfo fieldInfo = property.GetFieldInfo();
                StatsContainer statsContainer = fieldInfo.GetValue(property.serializedObject.targetObject) as StatsContainer;

                statsContainer.OnStatAdded.RemoveListener(OnStatAdded);
                statsContainer.OnStatRemoved.RemoveListener(OnStatRemoved);

                statsContainer.OnStatAdded.AddListener(OnStatAdded);
                statsContainer.OnStatRemoved.AddListener(OnStatRemoved);

                for (int i = 0; i < statsContainer.StatCollections.Count; i++)
                {
                    var collection = statsContainer.StatCollections[i];
                    VisualElement pf = CreateStatField(collection);

                    previewFoldout.Add(pf);
                }
            }
            #endregion

            return container;
        }

        private void OnStatAdded(StatCollection statCollection)
        {
            VisualElement pf = CreateStatField(statCollection);

            previewFoldout.Add(pf);
        }
        private void OnStatRemoved(Stat stat)
        {
            if (statFieldByStat.TryGetValue(stat, out var field))
            {
                previewFoldout.Remove(field);
                statFieldByStat.Remove(stat);
            }
        }

        private VisualElement CreateStatField(StatCollection statCollection)
        { 
            VisualElement statField = new VisualElement();
            statField.style.flexDirection = FlexDirection.Row;
            statField.style.justifyContent = Justify.SpaceBetween;

            Label statName = new Label(statCollection.Stat.DisplayName);
            statName.style.marginRight = 20;

            Label statValue = new Label(statCollection.TotalValue.ToString());
            statValue.style.marginRight = 250;

            statValue.schedule.Execute(() =>
            {
                statValue.text = statCollection.TotalValue.ToString();
            }).Every(100);
            
            statField.Add(statName);
            statField.Add(statValue);

            statFieldByStat.Add(statCollection.Stat, statField);

            return statField;
        }
    }
}