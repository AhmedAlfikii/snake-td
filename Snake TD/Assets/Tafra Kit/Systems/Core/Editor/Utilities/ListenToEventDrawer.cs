using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using TafraKit;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(ListenToEvent))]
    public class ListenToEventDrawer : PropertyDrawer
    {
        class ViewData
        {
            public SerializedProperty targetProperty;
            public SerializedProperty componentNameProperty;
            public SerializedProperty eventNameProperty;
            public SerializedProperty onEventFulfilledProperty;
            public SerializedProperty numberRelationProperty;
            public SerializedProperty numberToCompareAgainstProperty;
            public SerializedProperty stringRelationProperty;
            public SerializedProperty stringToCompareAgainstProperty;
            public SerializedProperty boolRelationProperty;
            public SerializedProperty selectedTypeProperty;
            public SerializedProperty isExpandedProperty;

            public List<SerializedProperty> properties = new List<SerializedProperty>();

            public bool isEnabled = false;
        }

        private Dictionary<string, ViewData> m_PerPropertyViewData = new Dictionary<string, ViewData>();

        private List<Type> acceptedTypes = new List<Type>()
        {
            typeof(UnityEngine.Events.UnityEvent),
            typeof(UnityEngine.Events.UnityEventBase),
            typeof(UnityEngine.Events.UnityEvent<int>),
            typeof(UnityEngine.Events.UnityEvent<float>),
            typeof(UnityEngine.Events.UnityEvent<string>),
            typeof(UnityEngine.Events.UnityEvent<bool>)
        };

        private bool eventSelected;
        private bool showNumberCondition;
        private bool showStringCondition;
        private bool showBoolCondition;

        void Enable(SerializedProperty property)
        {
            ViewData viewData;
            if (!m_PerPropertyViewData.TryGetValue(property.propertyPath, out viewData))
            {
                viewData = new ViewData();
                m_PerPropertyViewData[property.propertyPath] = viewData;
            }

            viewData.targetProperty = property.FindPropertyRelative("Target");
            viewData.componentNameProperty = property.FindPropertyRelative("ComponentName");
            viewData.eventNameProperty = property.FindPropertyRelative("EventName");
            viewData.onEventFulfilledProperty = property.FindPropertyRelative("OnEventFulfilled");
            viewData.numberRelationProperty = property.FindPropertyRelative("numberRelation");
            viewData.numberToCompareAgainstProperty = property.FindPropertyRelative("numberToCompareAgainst");
            viewData.stringRelationProperty = property.FindPropertyRelative("stringRelation");
            viewData.stringToCompareAgainstProperty = property.FindPropertyRelative("stringToCompareAgainst");
            viewData.boolRelationProperty = property.FindPropertyRelative("boolRelation");
            viewData.selectedTypeProperty = property.FindPropertyRelative("selectedType");
            viewData.isExpandedProperty = property.FindPropertyRelative("isExpanded");

            viewData.properties.Add(viewData.targetProperty);
            viewData.properties.Add(viewData.componentNameProperty);
            viewData.properties.Add(viewData.eventNameProperty);
            viewData.properties.Add(viewData.numberRelationProperty);
            viewData.properties.Add(viewData.onEventFulfilledProperty);

            viewData.isEnabled = true;

            CheckPropertiesToShow(viewData);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ViewData viewData;
            if (!m_PerPropertyViewData.TryGetValue(property.propertyPath, out viewData))
            {
                viewData = new ViewData();
                m_PerPropertyViewData[property.propertyPath] = viewData;
            }

            if (!viewData.isEnabled)
                Enable(property);

            EditorGUI.BeginProperty(position, label, property);

            Rect boxRect = position;

            Rect labelOutlineRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight + 2);

            GUI.Box(boxRect, "");

            //Header.
            Rect labelRect = new Rect(boxRect.x + 15, boxRect.y + 1, boxRect.width - 20, EditorGUIUtility.singleLineHeight);
            viewData.isExpandedProperty.boolValue = true;// EditorGUI.BeginFoldoutHeaderGroup(labelRect, viewData.isExpandedProperty.boolValue, property.displayName);
            EditorGUI.LabelField(labelRect, label);
            if (viewData.isExpandedProperty.boolValue)
            {
                //Change indent.
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indent + 0;

                //Rects.
                Rect targetRect = new Rect(labelRect.x, labelRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, labelRect.width, EditorGUI.GetPropertyHeight(viewData.targetProperty));
                Rect dropdownRect = new Rect(labelRect.x, targetRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, labelRect.width, EditorGUI.GetPropertyHeight(viewData.componentNameProperty));
                Rect relationRect = showNumberCondition || showStringCondition || showBoolCondition? new Rect(labelRect.x, dropdownRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, labelRect.width, EditorGUI.GetPropertyHeight(viewData.numberRelationProperty)) : dropdownRect;
                Rect onEventFulfilledRect = new Rect(labelRect.x, relationRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, labelRect.width, EditorGUI.GetPropertyHeight(viewData.onEventFulfilledProperty));

                List<string> dropdownOptions = new List<string>();
                GenericMenu menu = null;

                //Fields Drawing.
                EditorGUI.BeginChangeCheck();

                EditorGUI.PropertyField(targetRect, viewData.targetProperty);

                bool goExists = viewData.targetProperty.objectReferenceValue != null;

                if (EditorGUI.EndChangeCheck())
                {
                    viewData.eventNameProperty.stringValue = "";
                    viewData.componentNameProperty.stringValue = "";
                }

                if (goExists)
                {
                    //dropdownOptions = FindPopupList(targetProperty.objectReferenceValue);
                    menu = GetMenu(viewData.targetProperty.objectReferenceValue, viewData);
                }

                EditorGUI.BeginDisabledGroup(!goExists);

                string buttonContent = string.IsNullOrEmpty(viewData.eventNameProperty.stringValue) ? "No Event" : viewData.componentNameProperty.stringValue + "." + viewData.eventNameProperty.stringValue;
                
                Rect buttonRect = EditorGUI.PrefixLabel(dropdownRect, new GUIContent("Event"));

                if (GUI.Button(buttonRect, new GUIContent(buttonContent), EditorStyles.popup))
                    menu.DropDown(buttonRect);

                //Display event by selected type
                CheckPropertiesToShow(viewData);

                if (showNumberCondition || showStringCondition || showBoolCondition)
                {
                    Rect conditionLabelRect = new Rect(relationRect.x, relationRect.y, relationRect.width, relationRect.height);
                    conditionLabelRect = EditorGUI.PrefixLabel(conditionLabelRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Condition"));

                    Rect relationEnumRect = conditionLabelRect;
                    relationEnumRect.width /= 2;
                    relationEnumRect.width -= EditorGUIUtility.standardVerticalSpacing;
                    Rect comparingValueRect = relationEnumRect;
                    comparingValueRect.x += comparingValueRect.width + EditorGUIUtility.standardVerticalSpacing * 2;

                    if (showNumberCondition)
                    {
                        if (viewData.numberRelationProperty.enumValueIndex != 0)
                        {
                            EditorGUI.PropertyField(relationEnumRect, viewData.numberRelationProperty, GUIContent.none);
                            EditorGUI.PropertyField(comparingValueRect, viewData.numberToCompareAgainstProperty, GUIContent.none);
                        }
                        else
                            EditorGUI.PropertyField(conditionLabelRect, viewData.numberRelationProperty, GUIContent.none);
                    }
                    else if (showStringCondition)
                    {
                        if (viewData.stringRelationProperty.enumValueIndex != 0)
                        {
                            EditorGUI.PropertyField(relationEnumRect, viewData.stringRelationProperty, GUIContent.none);
                            EditorGUI.PropertyField(comparingValueRect, viewData.stringToCompareAgainstProperty, GUIContent.none);
                        }
                        else
                            EditorGUI.PropertyField(conditionLabelRect, viewData.stringRelationProperty, GUIContent.none);
                    }
                    else if (showBoolCondition)
                    {
                        EditorGUI.PropertyField(conditionLabelRect, viewData.boolRelationProperty, GUIContent.none);
                    }
                }

                if (eventSelected)
                    EditorGUI.PropertyField(onEventFulfilledRect, viewData.onEventFulfilledProperty);

                //Revert indent change.
                EditorGUI.indentLevel = indent;
            }

            EditorGUI.EndDisabledGroup();

            //EditorGUI.EndFoldoutHeaderGroup();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ViewData viewData;
            if (!m_PerPropertyViewData.TryGetValue(property.propertyPath, out viewData))
            {
                viewData = new ViewData();
                m_PerPropertyViewData[property.propertyPath] = viewData;
            }

            if (!viewData.isEnabled)
                Enable(property);

            float totalHeight = 0;
            if (viewData.isExpandedProperty.boolValue)
            {
                for (int i = 0; i < viewData.properties.Count; i++)
                {
                    if (!eventSelected && viewData.properties[i] == viewData.onEventFulfilledProperty)
                        continue;
                    if ((!showNumberCondition && !showStringCondition && !showBoolCondition) && viewData.properties[i] == viewData.numberRelationProperty)
                        continue;

                    totalHeight += EditorGUI.GetPropertyHeight(viewData.properties[i]);

                    if (i > 0)
                        totalHeight += EditorGUIUtility.standardVerticalSpacing;
                }
            }
            else
                totalHeight += EditorGUIUtility.singleLineHeight;

            //Add to cover the extra space added for the gui box.
            totalHeight += 2;

            return totalHeight;
        }

        GenericMenu GetMenu(UnityEngine.Object target, ViewData viewData)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("No Event"), string.IsNullOrEmpty(viewData.eventNameProperty.stringValue), ()=>{
                viewData.eventNameProperty.stringValue = "";
                viewData.selectedTypeProperty.enumValueIndex = 0;
                viewData.eventNameProperty.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            UnityEngine.Object finalTarget = target;

            //In case a component was assigned as the target, use its game Object instead.
            if (finalTarget is Component)
                finalTarget = ((Component)finalTarget).gameObject;

            AddMenuItemsForType(menu, finalTarget, viewData);

            if (finalTarget is GameObject)
            {
                Component[] components = ((GameObject)finalTarget).GetComponents(typeof(Component));

                for (int i = 0; i < components.Length; i++)
                {
                    AddMenuItemsForType(menu, components[i], viewData);
                }
            }

            return menu;
        }

        void AddMenuItemsForType(GenericMenu menu, UnityEngine.Object target, ViewData viewData)
        {
            Type targetType = target.GetType();

            FieldInfo[] fInfo = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            int compatibleEventsCount = 0;

            for (int j = 0; j < fInfo.Length; j++)
            {
                Type rootType;
                if (fInfo[j].FieldType.BaseType != null)
                    rootType = fInfo[j].FieldType.BaseType;
                else
                    rootType = fInfo[j].FieldType;

                if (acceptedTypes.Contains(rootType))
                {
                    string fieldName = fInfo[j].Name;
                    string path = targetType.Name + "/" + fieldName;

                    menu.AddItem(new GUIContent(path), (viewData.componentNameProperty.stringValue + "/"  + viewData.eventNameProperty.stringValue) == path, ()=> {
                        viewData.componentNameProperty.stringValue = targetType.Name;
                        viewData.eventNameProperty.stringValue = fieldName;
                        for (int i = 0; i < acceptedTypes.Count; i++)
                        {
                            if (rootType == acceptedTypes[i])
                            {
                                if (i > 0)
                                    viewData.selectedTypeProperty.enumValueIndex = i;
                                else
                                {
                                    //In case the type of this event is UnityEvent, then use the Void enum as well.
                                    viewData.selectedTypeProperty.enumValueIndex = 1;
                                }
                            }
                        }
                        viewData.eventNameProperty.serializedObject.ApplyModifiedProperties();
                    });
                    compatibleEventsCount++;
                }
            }
        }

        void CheckPropertiesToShow(ViewData viewData)
        {
            int selectedTypeIndex = viewData.selectedTypeProperty.enumValueIndex;
            eventSelected = true;
            showNumberCondition = false;
            showStringCondition = false;
            showBoolCondition = false;
            switch (selectedTypeIndex)
            {
                case 1: //Void
                    break;
                case 2: //Int
                    showNumberCondition = true;
                    break;
                case 3: //Float
                    showNumberCondition = true;
                    break;
                case 4: //String
                    showStringCondition = true;
                    break;
                case 5: //Bool
                    showBoolCondition = true;
                    break;
                default:
                    eventSelected = false;
                    break;
            }
        }
    }
}